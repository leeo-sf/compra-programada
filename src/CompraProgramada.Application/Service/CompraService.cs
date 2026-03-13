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
    private readonly ICotacaoService _cotacaoService;

    public CompraService(ILogger<CompraService> logger,
        IHistoricoExecucaoMotorService historicoExecucaoService,
        IClienteService clienteService,
        ICalendarioMotorCompraService calendarioMotorCompraService,
        IDistribuicaoService distribuicaoService,
        IImpostoRendaService impostoRendaService,
        IOrdemCompraService ordemCompraService,
        ICustodiaMasterService custodiaMasterService,
        ICotacaoService cotacaoService)
    {
        _logger = logger;
        _historicoExecucaoService = historicoExecucaoService;
        _clienteService = clienteService;
        _calendarioMotorCompraService = calendarioMotorCompraService;
        _distribuicaoService = distribuicaoService;
        _impostoRendaService = impostoRendaService;
        _ordemCompraService = ordemCompraService;
        _custodiaMasterService = custodiaMasterService;
        _cotacaoService = cotacaoService;
    }

    public async Task<Result<ExecutarCompraResponse>?> ExecutarCompraAsync(DateTime? date, CancellationToken cancellationToken)
    {
        if (date is null)
        {
            var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);
            if (!deveExecutarCompraHoje)
            {
                var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
                _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
                return null;
            }
        }

        var dataExecucao = date ?? DateTime.Now;

        var clientesAtivosResult = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);
        if (!clientesAtivosResult.IsSuccess)
            throw clientesAtivosResult.Exception;

        var clientesAtivos = clientesAtivosResult.Value;
        var qtdClientesAtivos = clientesAtivos.Count;

        _logger.LogInformation("{QuantidadeClientes} clientes ativos para processamento.", qtdClientesAtivos);

        var valorTotalConsolidado = clientesAtivos.Sum(cliente => cliente.ValorMensal / 3);

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", valorTotalConsolidado);

        var ordensCompraRegistradas = await _ordemCompraService.EmitirOrdensDeCompraAsync(valorTotalConsolidado, cancellationToken);
        if (!ordensCompraRegistradas.IsSuccess)
            throw ordensCompraRegistradas.Exception;

        _logger.LogInformation("Ordens de compra registradas.");

        var (distribuicoes, ativosAhComprar) = await _distribuicaoService.RealizarDistribuicoesAsync(clientesAtivos, dataExecucao, ordensCompraRegistradas.Value, cancellationToken);

        _logger.LogInformation("Distribuições para as custodias realizadas.");

        var resultResiduosCapturados = await _custodiaMasterService.CapturarResiduosDeCustodiaDistribuida(ativosAhComprar, distribuicoes, cancellationToken);
        if (!resultResiduosCapturados.IsSuccess)
            throw resultResiduosCapturados.Exception;

        _logger.LogInformation("Distribuição para as custodias realizada.");

        var qtdIrPublicadoResult = await _impostoRendaService.CalcularIRDedoDuro(distribuicoes, cancellationToken);
        if (!qtdIrPublicadoResult.IsSuccess)
            throw qtdIrPublicadoResult.Exception;

        _logger.LogInformation("Ir Dedo Duro calculado e publicado para {QtdClientes} clientes.", qtdIrPublicadoResult.Value);

        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoService.SalvarExecucaoAsync(new ExecucaoMotorCompraDto { DataReferencia = dataReferencia, DataExecucao = dataExecucao }, cancellationToken);

        _logger.LogInformation("Registrado histórico da execução na base de dados.");

        var distribuicoesDto = distribuicoes
            .GroupBy(grupo => new { grupo.ClienteId, grupo.Nome, grupo.ValorAporte })
            .Select(g => new DistribuicaoDto(
                Id: 0,
                Cpf: string.Empty,
                OrdemCompraId: 0,
                ContaGraficaId: 0,
                Ticker: string.Empty,
                QuantidadeAlocada: 0,
                ValorOperacao: 0,
                ContaGrafica: null!,
                Data: DateTime.Now,
                g.Key.ClienteId, g.Key.Nome, g.Key.ValorAporte, g.SelectMany(x => x.Ativos).ToList())).ToList();

        return new ExecutarCompraResponse(
            dataExecucao,
            qtdClientesAtivos,
            valorTotalConsolidado,
            ordensCompraRegistradas.Value,
            distribuicoesDto,
            resultResiduosCapturados.Value,
            qtdIrPublicadoResult.Value,
            $"Compra programada executada com sucesso para {qtdClientesAtivos} clientes.");
    }
}