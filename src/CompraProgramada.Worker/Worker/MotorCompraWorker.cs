using CompraProgramada.Application.Config;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Worker.Worker;

public class MotorCompraWorker : BackgroundService
{
    private readonly ILogger<MotorCompraWorker> _logger;
    private readonly AppConfig _appConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MotorCompraWorker(ILogger<MotorCompraWorker> logger,
        AppConfig appConfig,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _appConfig = appConfig;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodo = TimeSpan.FromHours(_appConfig.MotorCompraConfig?.TempoEmHoraAhCadaExecucao ?? 1);
        var timer = new PeriodicTimer(periodo);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ExecutarMotorDeCompra(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao executar o motor de compra.");
                throw;
            }
        }
    }

    internal async Task ExecutarMotorDeCompra(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o motor de compra...");

        using var scope = _serviceScopeFactory.CreateScope();

        var motorCompraService = scope.ServiceProvider.GetRequiredService<ICompraService>();

        await motorCompraService.ExecutarCompraAsync(null, cancellationToken);

        _logger.LogInformation("Motor de compra finalizado com sucesso.");
    }
}