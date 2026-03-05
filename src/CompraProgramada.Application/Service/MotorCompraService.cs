using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Service;

public class MotorCompraService : IMotorCompraService
{
    private readonly ILogger<MotorCompraService> _logger;
    private readonly IHistoricoExecucaoMotorService _historicoExecucaoService;
    private readonly IClienteService _clienteService;
    private readonly ICalendarioMotorCompraService _calendarioMotorCompraService;
    private readonly IDistribuicaoService _distribuicaoService;
    private readonly IImpostoRendaService _impostoRendaService;

    public MotorCompraService(ILogger<MotorCompraService> logger,
        IHistoricoExecucaoMotorService historicoExecucaoService,
        IClienteService clienteService,
        ICalendarioMotorCompraService calendarioMotorCompraService,
        IDistribuicaoService distribuicaoService,
        IImpostoRendaService impostoRendaService)
    {
        _logger = logger;
        _historicoExecucaoService = historicoExecucaoService;
        _clienteService = clienteService;
        _calendarioMotorCompraService = calendarioMotorCompraService;
        _distribuicaoService = distribuicaoService;
        _impostoRendaService = impostoRendaService;
    }

    public async Task ExecutarCompraAsync(CancellationToken cancellationToken)
    {
        var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);
        if (!deveExecutarCompraHoje)
        {
            var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
            _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
            return;
        }

        var clientesAtivos = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);
        if (!clientesAtivos.IsSuccess || !clientesAtivos.Value!.Any())
            throw new ApplicationException($"Erro ao obter clientes ativos. {clientesAtivos.Exception!.Message}");

        _logger.LogInformation("{QuantidadeClientes} clientes ativos para processamento.", clientesAtivos.Value!.Count);

        var totalConsolidadoResult = _clienteService.TotalConsolidade(clientesAtivos.Value);
        if (!totalConsolidadoResult.IsSuccess)
            throw new ApplicationException("Erro ao calcular total consolidado.");

        var valorTotalConsolidado = totalConsolidadoResult.Value;

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", valorTotalConsolidado);

        var distribuicoesGrupoClientes = await _distribuicaoService.RealizarDistribuicaoAtivoPorCliente(clientesAtivos.Value, valorTotalConsolidado, cancellationToken);

        if (!distribuicoesGrupoClientes.IsSuccess)
            throw new ApplicationException(distribuicoesGrupoClientes.Exception.Message);

        _logger.LogInformation("Distribuição de grupo realizada e residuos definidos.");

        await _impostoRendaService.CalcularIRDedoDuro(distribuicoesGrupoClientes.Value, cancellationToken);

        var dataExecucao = DateTime.Now;
        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoService.SalvarExecucaoAsync(new ExecucaoMotorCompraDto { DataReferencia = dataReferencia, DataExecucao = dataExecucao }, cancellationToken);

        _logger.LogInformation("Registrado histórico da execução na base de dados.");
    }
}