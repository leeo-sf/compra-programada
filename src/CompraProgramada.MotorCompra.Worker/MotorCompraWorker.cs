using CompraProgramada.Shared.Config;
using CompraProgramada.Shared.Extensions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.MotorCompra.Worker;

public class MotorCompraWorker : BackgroundService
{
    private readonly ILogger<MotorCompraWorker> _logger;
    private readonly MotorCompraConfig _motorConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MotorCompraWorker(
        ILogger<MotorCompraWorker> logger,
        AppConfig appConfig,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _motorConfig = appConfig.MotorCompraConfig;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodo = TimeSpan.FromHours(_motorConfig.TempoEmHoraAhCadaExecucao);
        var timer = new PeriodicTimer(periodo);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var result = await ExecutarMotorDeCompra(stoppingToken);

            if (!result.IsSuccess)
            {
                _logger.LogErroExecucaoMotorCompra(result.Exception);
                continue;
            }

            _logger.LogSucessoExecucaoMotorCompra(result.Value);
        }
    }

    internal async Task<Result<ExecutarMotorCompraResponse>> ExecutarMotorDeCompra(CancellationToken cancellationToken)
    {
        _logger.LogIniciandoMotorCompra();

        using var scope = _serviceScopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new ExecutarMotorCompraRequest(default), cancellationToken);

        if (!response.IsSuccess)
            return response.Exception;

        return response;
    }
}