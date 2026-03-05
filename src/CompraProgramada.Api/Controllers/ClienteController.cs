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

    [HttpGet("{clienteId}/carteira")]
    public async Task<ActionResult<CarteiraCustodiaResponse>> CustodiaCarteiraAsync(int clienteId)
        => await SendCommand(new CarteiraCustodiaRequest(clienteId));

    [HttpPost("{clienteId}/saida")]
    public async Task<ActionResult<SaidaProdutoResponse>> SaidaProdutoAsync(int clienteId)
        => await SendCommand(new SaidaProdutoRequest(clienteId));

    [HttpPut("{clienteId}/valor-mensal")]
    public async Task<ActionResult<AtualizarValorMensalResponse>> AlterarValorMensalAsync(int clienteId, AtualizarValorMensalRequest request)
        => await SendCommand(request with { ClienteId = clienteId });
}