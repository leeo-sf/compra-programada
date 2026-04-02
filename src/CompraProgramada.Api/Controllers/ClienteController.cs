using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompraProgramada.Api.Controllers;

[Route("api/clientes")]
public class ClienteController(IMediator mediator) : BaseController(mediator)
{
    /// <summary>
    /// Aderir ao produto (cesta oferecida atual)
    /// </summary>
    [HttpPost("adesao")]
    public async Task<ActionResult<AdesaoResponse>> AdesaoAsync(AdesaoRequest request)
        => await SendCommand(request, 201);

    /// <summary>
    /// Abandonar o produto
    /// </summary>
    [HttpPost("{clienteId}/saida")]
    public async Task<ActionResult<SaidaProdutoResponse>> SaidaProdutoAsync(int clienteId)
        => await SendCommand(new SaidaProdutoRequest(clienteId));

    /// <summary>
    /// Atualizar valor mensal de compra
    /// </summary>
    [HttpPut("{clienteId}/valor-mensal")]
    public async Task<ActionResult<AtualizarValorMensalResponse>> AlterarValorMensalAsync(int clienteId, AtualizarValorMensalRequest request)
        => await SendCommand(request with { ClienteId = clienteId });

    /// <summary>
    /// Consutar custodias
    /// </summary>
    [HttpGet("{clienteId}/carteira")]
    public async Task<ActionResult<CarteiraCustodiaResponse>> CustodiaCarteiraAsync(int clienteId)
        => await SendCommand(new CarteiraCustodiaRequest(clienteId));

    /// <summary>
    /// Consultar rentabilidade da carteita detalhada
    /// </summary>
    [HttpGet("{clienteId}/rentabilidade")]
    public async Task<ActionResult<RentabilidadeResponse>> RentabilidadeAsync(int clienteId)
        => await SendCommand(new RentabilidadeRequest(clienteId));
}