using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using Microsoft.Extensions.Logging;

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

    public async Task ExecutarCompraAsync(CancellationToken cancellationToken)
    {
        /*var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);
        if (!deveExecutarCompraHoje)
        {
            var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
            _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
            return;
        }*/

        var clientesAtivos = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);
        if (!clientesAtivos.IsSuccess)
            throw clientesAtivos.Exception;

        _logger.LogInformation("{QuantidadeClientes} clientes ativos para processamento.", clientesAtivos.Value!.Count);

        var valorTotalConsolidadoResult = _clienteService.TotalConsolidade(clientesAtivos.Value);
        if (!valorTotalConsolidadoResult.IsSuccess)
            throw valorTotalConsolidadoResult.Exception;

        var valorTotalConsolidado = valorTotalConsolidadoResult.Value;

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", valorTotalConsolidado);

        var (grupoAtivosDistribuido, ordensCompraEmitidas) = await _distribuicaoService.RealizaDistribuicaoGrupoAtivo(clientesAtivos.Value, valorTotalConsolidado, cancellationToken);

        // DEFINIR LOTE

        _logger.LogInformation("Calculo de quantidade de realizados ativos a comprar: {Grupos}", grupoAtivosDistribuido);

        var ordensCompraRegistradas = await _ordemCompraService.RegistrarOrdensDeCompraAsync(ordensCompraEmitidas, cancellationToken);
        if (!ordensCompraRegistradas.IsSuccess)
            throw ordensCompraRegistradas.Exception;

        _logger.LogInformation("Ordens de compra geradas e salvas: {OrdensCompra}", ordensCompraRegistradas.Value);

        var distribuicaoResult = await _distribuicaoService.DistribuirCustodiasPorAtivo(clientesAtivos.Value, grupoAtivosDistribuido, valorTotalConsolidado, cancellationToken);
        if (!distribuicaoResult.IsSuccess)
            throw distribuicaoResult.Exception;

        await _distribuicaoService.SalvarRegistroDistribuicoes(distribuicaoResult.Value, ordensCompraRegistradas.Value, cancellationToken);

        _logger.LogInformation("Distribuição para as custodias realizada.");

        var resultResiduosCapturados = await _custodiaMasterService.CapturarResiduosDeCustodiaDistribuida(grupoAtivosDistribuido, distribuicaoResult.Value, cancellationToken);
        if (!resultResiduosCapturados.IsSuccess)
            throw resultResiduosCapturados.Exception;

        _logger.LogInformation("Distribuição para as custodias realizada.");

        await _impostoRendaService.CalcularIRDedoDuro(distribuicaoResult.Value, cancellationToken);

        var dataExecucao = DateTime.Now;
        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoService.SalvarExecucaoAsync(new ExecucaoMotorCompraDto { DataReferencia = dataReferencia, DataExecucao = dataExecucao }, cancellationToken);

        _logger.LogInformation("Registrado histórico da execução na base de dados.");
    }

    public Task SeparacaoLoteDeCompra()
    {
        throw new NotImplementedException();
    }
}