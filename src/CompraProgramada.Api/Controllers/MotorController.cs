using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompraProgramada.Api.Controllers;

[Route("api/motor")]
public class MotorController(IMediator mediator) : BaseController(mediator)
{
    /// <summary>
    /// Dispara a execução do motor de compra (utilizado para testes)
    /// </summary>
    [HttpPost("executar-compra")]
    public async Task<ActionResult<ExecutarCompraResponse>> ExecutarCompraAsync(ExecutarCompraRequest request)
        => await SendCommand(request);
}