using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Worker.Worker;

public class MotorDeCompraWorker : BackgroundService
{
    private readonly ILogger<MotorDeCompraWorker> _logger;
    private readonly AppConfig _appConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICotahistParser _cotahistParser;

    public MotorDeCompraWorker(ILogger<MotorDeCompraWorker> logger,
        AppConfig appConfig,
        IServiceScopeFactory serviceScopeFactory,
        ICotahistParser cotahistParser)
    {
        _logger = logger;
        _appConfig = appConfig;
        _serviceScopeFactory = serviceScopeFactory;
        _cotahistParser = cotahistParser;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //var timer = new PeriodicTimer(TimeSpan.FromHours(_appConfig.MotorCompra.TempoEmHoraAhCadaExecucao));
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var motorCompraService = scope.ServiceProvider.GetRequiredService<IMotorCompraService>();

                await motorCompraService.ExecutarCompraAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um problema ao executar as compras");
                throw;
            }
        }
    }
}