using CompraProgramada.Api.Controllers.Base;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompraProgramada.Api.Controllers;

[Route("api/motor")]
public class MotorController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("executar-compra")]
    public async Task<ActionResult<ExecutarCompraResponse>> ExecutarCompraAsync(ExecutarCompraRequest request)
        => await SendCommand(request);
}