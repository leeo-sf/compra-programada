using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Handler;

public class MotorCompraHandler : IRequestHandler<ExecutarCompraRequest, Result<ExecutarCompraResponse>>
{
    private readonly ILogger<MotorCompraHandler> _logger;
    private readonly ICompraService _compraService;

    public MotorCompraHandler(ILogger<MotorCompraHandler> logger,
        ICompraService compraService)
    {
        _logger = logger;
        _compraService = compraService;
    }

    public async Task<Result<ExecutarCompraResponse>> Handle(ExecutarCompraRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitado execucação do motor de compra com a data {Data}", request);

        var compraResult = await _compraService.ExecutarCompraAsync(request.DataReferencia.ToDateTime(TimeOnly.MinValue), cancellationToken);
        if (!compraResult.IsSuccess)
            return compraResult.Exception;

        return compraResult;
    }
}
