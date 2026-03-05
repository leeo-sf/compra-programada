using CompraProgramada.Api.Controllers.Base;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompraProgramada.Api.Controllers;

[Route("api/clientes")]
public class ClienteController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("adesao")]
    public async Task<ActionResult<AdesaoResponse>> AdesaoAsync(AdesaoRequest request)
        => await SendCommand(request, 201);

    [HttpPost("{clienteId}/saida")]
    public async Task<ActionResult<SaidaProdutoResponse>> AlterarValorMensalAsync(int clienteId)
        => await SendCommand(new SaidaProdutoRequest(clienteId));
}