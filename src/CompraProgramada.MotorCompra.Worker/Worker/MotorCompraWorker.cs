using CompraProgramada.Shared.Config;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Worker.Worker;

public class MotorCompraWorker : BackgroundService
{
    private readonly ILogger<MotorCompraWorker> _logger;
    private readonly AppConfig _appConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MotorCompraWorker(
        ILogger<MotorCompraWorker> logger,
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
            var result = await ExecutarMotorDeCompra(stoppingToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao executar o motor de compra. {Exception}", result.Exception);
                continue;
            }

            _logger.LogInformation("Motor de compra executado com sucesso {Result}.", result.Value);
        }
    }

    internal async Task<Result<ExecutarMotorCompraResponse>> ExecutarMotorDeCompra(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o motor de compra...");

        using var scope = _serviceScopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new ExecutarMotorCompraRequest(default), cancellationToken);

        if (!response.IsSuccess)
            return response.Exception;

        return response;
    }
}