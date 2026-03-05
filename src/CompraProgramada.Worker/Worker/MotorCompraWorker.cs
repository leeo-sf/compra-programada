using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;

namespace CompraProgramada.Worker.Worker;

public class MotorCompraWorker : BackgroundService
{
    private readonly ILogger<MotorCompraWorker> _logger;
    private readonly AppConfig _appConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICotahistParserService _cotahistParser;

    public MotorCompraWorker(ILogger<MotorCompraWorker> logger,
        AppConfig appConfig,
        IServiceScopeFactory serviceScopeFactory,
        ICotahistParserService cotahistParser)
    {
        _logger = logger;
        _appConfig = appConfig;
        _serviceScopeFactory = serviceScopeFactory;
        _cotahistParser = cotahistParser;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodo = TimeSpan.FromHours(_appConfig.MotorCompra?.TempoEmHoraAhCadaExecucao ?? 2);
        var timer = new PeriodicTimer(periodo);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Iniciando o MotorCompra...");

                using var scope = _serviceScopeFactory.CreateScope();

                var motorCompraService = scope.ServiceProvider.GetRequiredService<ICompraService>();

                await motorCompraService.ExecutarCompraAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um um erro ao executar o motor de compras.");
                throw;
            }

            _logger.LogInformation("MotorCompra finalizado com sucesso.");
        }
    }
}