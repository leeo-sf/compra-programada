using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
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
    private readonly OrdemCompraMapper _mapperOrdemCompra;
    private readonly DistribuicaoMapper _distribuicaoMapper;

    public CompraService(ILogger<CompraService> logger,
        IHistoricoExecucaoMotorService historicoExecucaoService,
        IClienteService clienteService,
        ICalendarioMotorCompraService calendarioMotorCompraService,
        IDistribuicaoService distribuicaoService,
        IImpostoRendaService impostoRendaService,
        IOrdemCompraService ordemCompraService,
        ICustodiaMasterService custodiaMasterService,
        OrdemCompraMapper mapperOrdemCompra,
        DistribuicaoMapper distribuicaoMapper)
    {
        _logger = logger;
        _historicoExecucaoService = historicoExecucaoService;
        _clienteService = clienteService;
        _calendarioMotorCompraService = calendarioMotorCompraService;
        _distribuicaoService = distribuicaoService;
        _impostoRendaService = impostoRendaService;
        _ordemCompraService = ordemCompraService;
        _custodiaMasterService = custodiaMasterService;
        _mapperOrdemCompra = mapperOrdemCompra;
        _distribuicaoMapper = distribuicaoMapper;
    }

    public async Task<Result<ExecutarCompraResponse?>> ExecutarCompraAsync(DateTime? date, CancellationToken cancellationToken)
    {
        if (date is null)
        {
            var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);
            if (!deveExecutarCompraHoje)
            {
                var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
                _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
                return Result.Success<ExecutarCompraResponse>(null!)!;
            }
        }

        var dataExecucao = date ?? DateTime.Now;

        var clientesAtivosResult = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);
        if (!clientesAtivosResult.IsSuccess)
            return clientesAtivosResult.Exception;

        var clientesAtivos = clientesAtivosResult.Value;
        var qtdClientesAtivos = clientesAtivos.Count;

        if (qtdClientesAtivos < 1)
            return new CompraException("Nenhum cliente ativo cadastrado", "QTD_CLIENTES_ATIVOS");

        _logger.LogInformation("{QuantidadeClientes} clientes ativos para processamento.", qtdClientesAtivos);

        var valorTotalConsolidado = clientesAtivos.Sum(cliente => cliente.ValorAporte);

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", valorTotalConsolidado);

        var ordensCompraResult = await _ordemCompraService.EmitirOrdensDeCompraAsync(valorTotalConsolidado, cancellationToken);
        if (!ordensCompraResult.IsSuccess)
            return ordensCompraResult.Exception;

        var distribuicoesResult = await _distribuicaoService.DistribuirParaCustodiasAsync(clientesAtivos, ordensCompraResult.Value, dataExecucao, cancellationToken);
        if (!distribuicoesResult.IsSuccess)
            return distribuicoesResult.Exception;

        _logger.LogInformation("Distribuições para as custodias realizadas.");

        var distribuicoes = distribuicoesResult.Value;

        var residuosResult = await _custodiaMasterService.CapturarResiduosNaoDistribuidosAsync(distribuicoes, ordensCompraResult.Value, cancellationToken);
        if (!residuosResult.IsSuccess)
            return residuosResult.Exception;

        var qtdIrPublicadoResult = await _impostoRendaService.CalcularIRDedoDuro(distribuicoes, cancellationToken);
        if (!qtdIrPublicadoResult.IsSuccess)
            return qtdIrPublicadoResult.Exception;

        _logger.LogInformation("Ir Dedo Duro calculado e publicado para {QtdClientes} clientes.", qtdIrPublicadoResult.Value);

        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoService.SalvarExecucaoAsync(dataReferencia, dataExecucao, cancellationToken);

        _logger.LogInformation("Registrado histórico da execução do motor de compra na base de dados.");

        return new ExecutarCompraResponse(
            dataExecucao,
            qtdClientesAtivos,
            valorTotalConsolidado,
            _mapperOrdemCompra.ToResponse(ordensCompraResult.Value),
            GerarDistribuicoesDtoResponse(distribuicoes),
            residuosResult.Value,
            qtdIrPublicadoResult.Value,
            $"Compra programada executada com sucesso para {qtdClientesAtivos} clientes.");
    }

    private List<DistribuicaoDto> GerarDistribuicoesDtoResponse(List<Distribuicao> distribuicoes)
    {
        var distribuicoesDto = _distribuicaoMapper.ToResponse(distribuicoes);

        return distribuicoesDto.GroupBy(grupo => new { grupo.ClienteId, grupo.Nome, grupo.ValorAporte })
            .Select(g => new DistribuicaoDto
            {
                Id = 0,
                Cpf = string.Empty,
                OrdemCompraId = 0,
                ContaGraficaId = 0,
                Ticker = string.Empty,
                QuantidadeAlocada = 0,
                ValorOperacao = 0,
                ContaGrafica = null!,
                Data = DateTime.Now,
                ClienteId = g.Key.ClienteId,
                Nome = g.Key.Nome,
                ValorAporte = g.Key.ValorAporte,
                Ativos = g.SelectMany(x => x.Ativos).ToList()
            }).ToList();
    }
}