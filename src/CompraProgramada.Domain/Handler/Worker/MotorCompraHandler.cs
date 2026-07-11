using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Domain.Handler.Worker;

public class MotorCompraHandler : IRequestHandler<ExecutarMotorCompraRequest, Result<ExecutarMotorCompraResponse>>
{
    private readonly ILogger<MotorCompraHandler> _logger;
    private readonly IHistoricoExecucaoMotorRepository _historicoExecucaoMotorRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICalendarioMotorCompraService _calendarioMotorCompraService;
    private readonly IImpostoRendaService _impostoRendaService;
    private readonly IOrdemCompraService _ordemCompraService;
    private readonly ICustodiaMasterRepository _custodiaMasterRepository;
    private readonly OrdemCompraMapper _mapperOrdemCompra;
    private readonly DistribuicaoMapper _distribuicaoMapper;
    private readonly IDateTimeProvaider _dateTimeProvaider;

    public MotorCompraHandler(
        ILogger<MotorCompraHandler> logger,
        IHistoricoExecucaoMotorRepository historicoExecucaoMotorRepository,
        IClienteRepository clienteRepository,
        ICalendarioMotorCompraService calendarioMotorCompraService,
        IImpostoRendaService impostoRendaService,
        IOrdemCompraService ordemCompraService,
        ICustodiaMasterRepository custodiaMasterRepository,
        OrdemCompraMapper mapperOrdemCompra,
        DistribuicaoMapper distribuicaoMapper,
        IDateTimeProvaider dateTimeProvaider)
    {
        _logger = logger;
        _historicoExecucaoMotorRepository = historicoExecucaoMotorRepository;
        _clienteRepository = clienteRepository;
        _calendarioMotorCompraService = calendarioMotorCompraService;
        _impostoRendaService = impostoRendaService;
        _ordemCompraService = ordemCompraService;
        _custodiaMasterRepository = custodiaMasterRepository;
        _mapperOrdemCompra = mapperOrdemCompra;
        _distribuicaoMapper = distribuicaoMapper;
        _dateTimeProvaider = dateTimeProvaider;
    }

    public async Task<Result<ExecutarMotorCompraResponse>> Handle(ExecutarMotorCompraRequest request, CancellationToken cancellationToken)
    {
        if (!request.DataReferencia.HasValue)
        {
            var deveExecutarCompraHoje = await _calendarioMotorCompraService.DeveExecutarCompraHoje(cancellationToken);
            if (!deveExecutarCompraHoje)
            {
                var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
                _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
                return Result.Success<ExecutarMotorCompraResponse>(null!)!;
            }
        }

        var dataExecucao = request.DataReferencia?.ToDateTime(TimeOnly.MaxValue) ?? _dateTimeProvaider.Now;

        var clientes = await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);
        if (clientes is null)
            return new CompraException("Nenhum cliente ativo cadastrado", "QTD_CLIENTES_ATIVOS");

        var qtdClientesAtivos = clientes.Count;

        _logger.LogInformation("{QuantidadeClientes} clientes ativos para processamento.", qtdClientesAtivos);

        var valorTotalConsolidado = clientes.Sum(cliente => cliente.ValorAporte);

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", valorTotalConsolidado);

        var ordensCompraResult = await _ordemCompraService.EmitirOrdensDeCompraAsync(valorTotalConsolidado, cancellationToken);
        if (!ordensCompraResult.IsSuccess)
            return ordensCompraResult.Exception;

        var distribuicoesResult = await RealizaDistribuicoesEntreCustodias(clientes, ordensCompraResult.Value, dataExecucao, cancellationToken);
        if (!distribuicoesResult.IsSuccess)
            return distribuicoesResult.Exception;

        _logger.LogInformation("Distribuições para as custodias realizadas.");

        var distribuicoes = distribuicoesResult.Value;

        var residuosResult = await AtualizaAtivosNaoDistribuidosAsync(distribuicoes, ordensCompraResult.Value, cancellationToken);
        if (!residuosResult.IsSuccess)
            return residuosResult.Exception;

        var qtdIrPublicadoResult = await _impostoRendaService.PublicarIR(distribuicoes, cancellationToken);
        if (!qtdIrPublicadoResult.IsSuccess)
            return qtdIrPublicadoResult.Exception;

        _logger.LogInformation("Ir Dedo Duro calculado e publicado para {QtdClientes} clientes.", qtdIrPublicadoResult.Value);

        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoMotorRepository.CriarHistoricoExecucaoAsync(HistoricoExecucaoMotor.CriarRegistroHistorico(dataReferencia, dataExecucao), cancellationToken);

        _logger.LogInformation("Registrado histórico da execução do motor de compra na base de dados.");

        return new ExecutarMotorCompraResponse(
            dataExecucao,
            qtdClientesAtivos,
            valorTotalConsolidado,
            _mapperOrdemCompra.ToResponse(ordensCompraResult.Value),
            GerarDistribuicoesDtoResponse(distribuicoes),
            residuosResult.Value.Select(x => new AtivoQuantidadeDto { Ticker = x.Ticker, Quantidade = x.QuantidadeResiduo }).ToList(),
            qtdIrPublicadoResult.Value,
            $"Compra programada executada com sucesso para {qtdClientesAtivos} clientes.");
    }

    internal async Task<Result<List<Distribuicao>>> RealizaDistribuicoesEntreCustodias(List<Cliente> clientesAtivos, List<OrdemCompra> ordensCompra, DateTime dataExecucao, CancellationToken cancellationToken)
    {
        var valorTotalAportes = clientesAtivos.Sum(cliente => cliente.ValorMensal / 3);
        var residuosNaoDistribuidos = await _custodiaMasterRepository.ObterResiduosAsync(cancellationToken);

        _logger.LogInformation("Resíduos não distribuídos obtidos para reaproveitamento na distribuição: {Residuos}", residuosNaoDistribuidos);

        foreach (var ativo in ordensCompra)
        {
            int residuoCustodiaMaster = residuosNaoDistribuidos?.FirstOrDefault(r => r.Ticker == ativo.Ticker)?.QuantidadeResiduo ?? 0;
            var qtdTotalAtivoParaDistribuicao = ativo.QuantidadeTotal + residuoCustodiaMaster;

            foreach (var cliente in clientesAtivos.OrderByDescending(x => x.Id))
            {
                var contaCliente = cliente.ContaGrafica;
                var custodiaCliente = contaCliente.CustodiaFilhotes.FirstOrDefault(x => x.Ticker == ativo.Ticker && x.ContaGraficaId == contaCliente.Id);
                if (custodiaCliente is null)
                {
                    _logger.LogWarning("Ativo {Ticker} não encontrado para a conta {NumeroConta}. A distribuição deste ativo não será realizada para esta conta.", ativo.Ticker, contaCliente.NumeroConta);
                    continue;
                }

                var novaQuantidadeDeAtivos = (int)Math.Truncate(qtdTotalAtivoParaDistribuicao * (cliente.ValorAporte / valorTotalAportes));

                var valorPrecoMedio = custodiaCliente.CalcularPrecoMedio(ativo.PrecoUnitario, novaQuantidadeDeAtivos);

                custodiaCliente.AdicionarNovaQuantidade(novaQuantidadeDeAtivos);

                contaCliente.AdicionarCompra(HistoricoCompra.RegistrarHistorico(
                    contaCliente.Id,
                    ativo.Ticker,
                    novaQuantidadeDeAtivos,
                    ativo.PrecoUnitario,
                    valorPrecoMedio,
                    cliente.ValorAporte,
                    DateOnly.FromDateTime(dataExecucao)));

                contaCliente.AdicionarDistribuicao(
                    Distribuicao.CriarDistribuicao(novaQuantidadeDeAtivos, contaCliente, ativo));
            }
        }

        var contas = clientesAtivos.Select(x => x.ContaGrafica).ToList();
        var contasAtualizadas = await _clienteRepository.AtualizarContasAsync(contas, cancellationToken);

        _logger.LogInformation("Atualização realizada das nas contas que tiveram a distribuição na base de dados.");

        return contasAtualizadas.SelectMany(x => x.Distribuicoes).ToList();
    }

    internal async Task<Result<List<CustodiaMaster>>> AtualizaAtivosNaoDistribuidosAsync(List<Distribuicao> distribuicoes, List<OrdemCompra> ordensCompra, CancellationToken cancellationToken)
    {
        var custodias = await _custodiaMasterRepository.ObterResiduosAsync(cancellationToken);

        foreach (var ativo in ordensCompra)
        {
            var custodia = custodias?.FirstOrDefault(x => x.Ticker == ativo.Ticker);

            if (custodia is null)
            {
                custodia = CustodiaMaster.CriarCustodia(1, ativo.Ticker);
                custodias?.Add(custodia);
            }

            var qtdUtilizada = distribuicoes.Where(x => x.Ticker == ativo.Ticker)
                .Sum(x => x.QuantidadeAlocada);

            custodia?.AtualizarResiduo(ativo.QuantidadeTotal, qtdUtilizada);
        }

        await _custodiaMasterRepository.AtualizarResiduosAysnc(custodias!, cancellationToken);

        return custodias!;
    }

    private List<DistribuicaoDto> GerarDistribuicoesDtoResponse(List<Distribuicao> distribuicoes)
    {
        var distribuicoesDto = _distribuicaoMapper.ToResponse(distribuicoes);

        return [.. distribuicoesDto.GroupBy(grupo => new { grupo.ClienteId, grupo.Nome, grupo.ValorAporte })
            .Select(g => new DistribuicaoDto
            {
                Data = _dateTimeProvaider.Now,
                ClienteId = g.Key.ClienteId,
                Nome = g.Key.Nome,
                ValorAporte = g.Key.ValorAporte,
                Ativos = [.. g.SelectMany(x => x.Ativos)]
            })];
    }
}