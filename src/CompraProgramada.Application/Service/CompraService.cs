using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Response;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CompraService : ICompraService
{
    private readonly ILogger<CompraService> _logger;
    private readonly IHistoricoExecucaoMotorService _historicoExecucaoService;
    private readonly IClienteService _clienteService;
    private readonly ICalendarioMotorCompraService _calendarioMotorCompraService;
    private readonly IDistribuicaoService _distribuicaoService;
    private readonly IImpostoRendaService _impostoRendaService;
    private readonly IOrdemCompraService _ordemCompraService;
    private readonly ICustodiaMasterService _custodiaMasterService;

    public CompraService(ILogger<CompraService> logger,
        IHistoricoExecucaoMotorService historicoExecucaoService,
        IClienteService clienteService,
        ICalendarioMotorCompraService calendarioMotorCompraService,
        IDistribuicaoService distribuicaoService,
        IImpostoRendaService impostoRendaService,
        IOrdemCompraService ordemCompraService,
        ICustodiaMasterService custodiaMasterService)
    {
        _logger = logger;
        _historicoExecucaoService = historicoExecucaoService;
        _clienteService = clienteService;
        _calendarioMotorCompraService = calendarioMotorCompraService;
        _distribuicaoService = distribuicaoService;
        _impostoRendaService = impostoRendaService;
        _ordemCompraService = ordemCompraService;
        _custodiaMasterService = custodiaMasterService;
    }

    public async Task<Result<ExecutarCompraResponse>> ExecutarCompraAsync(DateTime? date, CancellationToken cancellationToken)
    {
        /*if (date is null)
        {
            var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);
            if (!deveExecutarCompraHoje)
            {
                var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
                _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
                return;
            }
        }*/

        var dataExecucao = date ?? DateTime.Now;

        var clientesAtivos = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);
        if (!clientesAtivos.IsSuccess)
            throw clientesAtivos.Exception;

        _logger.LogInformation("{QuantidadeClientes} clientes ativos para processamento.", clientesAtivos.Value!.Count);

        var valorTotalConsolidadoResult = _clienteService.TotalConsolidade(clientesAtivos.Value);
        if (!valorTotalConsolidadoResult.IsSuccess)
            throw valorTotalConsolidadoResult.Exception;

        var valorTotalConsolidado = valorTotalConsolidadoResult.Value;

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", valorTotalConsolidado);

        var (grupoAtivosDistribuido, fechamentos) = await _distribuicaoService.DistribuirGrupoAtivoAsync(clientesAtivos.Value, valorTotalConsolidado, dataExecucao, cancellationToken);

        _logger.LogInformation("Calculo de quantidade de realizados ativos a comprar: {Grupos}", grupoAtivosDistribuido);

        var ordensCompraResult = await SeparacaoLoteDeCompraAsync(fechamentos, dataExecucao, cancellationToken);
        if (!ordensCompraResult.IsSuccess)
            throw ordensCompraResult.Exception;

        _logger.LogInformation("Ordens de compra geradas: {OrdensCompra}", ordensCompraResult);

        var ordensCompraRegistradas = await _ordemCompraService.RegistrarOrdensDeCompraAsync(ordensCompraResult.Value, cancellationToken);
        if (!ordensCompraRegistradas.IsSuccess)
            throw ordensCompraRegistradas.Exception;

        _logger.LogInformation("Ordens de compra registradas.");

        var distribuicaoResult = await _distribuicaoService.DistribuirCustodiasAsync(clientesAtivos.Value, grupoAtivosDistribuido, valorTotalConsolidado, dataExecucao, cancellationToken);
        if (!distribuicaoResult.IsSuccess)
            throw distribuicaoResult.Exception;

        await _distribuicaoService.SalvarDistribuicoesAsync(distribuicaoResult.Value, ordensCompraRegistradas.Value, cancellationToken);

        _logger.LogInformation("Distribuição para as custodias realizada.");

        var resultResiduosCapturados = await _custodiaMasterService.CapturarResiduosDeCustodiaDistribuida(grupoAtivosDistribuido, distribuicaoResult.Value, cancellationToken);
        if (!resultResiduosCapturados.IsSuccess)
            throw resultResiduosCapturados.Exception;

        _logger.LogInformation("Distribuição para as custodias realizada.");

        var qtdIrPublicadoResult = await _impostoRendaService.CalcularIRDedoDuro(distribuicaoResult.Value, cancellationToken);
        if (!qtdIrPublicadoResult.IsSuccess)
            throw qtdIrPublicadoResult.Exception;

        _logger.LogInformation("Ir Dedo Duro calculado e publicado para {QtdClientes} clientes.", qtdIrPublicadoResult.Value);

        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoService.SalvarExecucaoAsync(new ExecucaoMotorCompraDto { DataReferencia = dataReferencia, DataExecucao = dataExecucao }, cancellationToken);

        _logger.LogInformation("Registrado histórico da execução na base de dados.");

        var totalClientes = clientesAtivos.Value.Count;
        List<DistribuicaoDto> distribuicoes = distribuicaoResult.Value
            .GroupBy(grupo => new { grupo.ClienteId, grupo.Nome, grupo.ValorAporte })
            .Select(g => new DistribuicaoDto(
                Id: 0,
                Cpf: string.Empty,
                OrdemCompraId: 0,
                ContaGraficaId: 0,
                Ticker: string.Empty,
                QuantidadeAlocada: 0,
                ValorOperacao: 0,
                ContaGrafica: default,
                Data: DateTime.Now,
                g.Key.ClienteId, g.Key.Nome, g.Key.ValorAporte, g.SelectMany(x => x.Ativos).ToList())).ToList();

        return new ExecutarCompraResponse(
            dataExecucao,
            totalClientes,
            valorTotalConsolidado,
            ordensCompraRegistradas.Value,
            distribuicoes,
            resultResiduosCapturados.Value,
            qtdIrPublicadoResult.Value,
            $"Compra programada executada com sucesso para {totalClientes} clientes.");
    }

    public async Task<Result<List<OrdemCompraDto>>> SeparacaoLoteDeCompraAsync(List<FechamentoAtivoB3Dto> fechamentoAtivos, DateTime dataExecucao, CancellationToken cancellationToken)
    {
        if (!fechamentoAtivos.Any())
            return new ApplicationException("É necessário os dados de fechamento da B3 para emitir as ordens de compra.");

        var residuosNaoDistribuidos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosNaoDistribuidos.IsSuccess)
            throw residuosNaoDistribuidos.Exception;

        var ordensCompra = new List<OrdemCompraDto>();

        foreach (var fechamento in fechamentoAtivos)
        {
            var custodia = residuosNaoDistribuidos.Value.FirstOrDefault(x => x.Ticker == fechamento.Ticker)!;
            var residuosAtuais = custodia.QuantidadeResiduos;
            var qtdNecessariaParaDistribuicao = (int)Math.Truncate(fechamento.ValorAhCompra / fechamento.PrecoFechamento);

            var quantidadeDeCompraAtivo = _custodiaMasterService.SubtrairResiduosParaCompra(custodia!, qtdNecessariaParaDistribuicao);

            var multiplosPresente = Math.DivRem(quantidadeDeCompraAtivo, 100, out int restos);
            var detalhes = new List<DetalheOrdemCompraDto>() { new DetalheOrdemCompraDto("FRACIONARIO", $"{fechamento.Ticker}F", restos) };

            if (multiplosPresente > 0)
                detalhes.Add(new DetalheOrdemCompraDto("PADRAO", fechamento.Ticker, multiplosPresente * 100));


            ordensCompra.Add(new OrdemCompraDto(
                0,
                fechamento.Ticker,
                quantidadeDeCompraAtivo,
                detalhes,
                fechamento.PrecoFechamento
            ));
        }

        return ordensCompra;
    }
}