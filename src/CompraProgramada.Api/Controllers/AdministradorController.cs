using CompraProgramada.Api.Controllers.Base;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompraProgramada.Api.Controllers;

[Route("api/admin")]
public class AdministradorController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("cesta")]
    public async Task<ActionResult<CriarAlterarCestaResponse>> CestaAsync(CriarAlterarCestaRequest request)
        => await SendCommand(request, 201);

    [HttpGet("cesta/atual")]
    public async Task<ActionResult<CestaResponse>> CestaAtualAsync()
        => await SendCommand(new CestaAtualRequest());

    [HttpGet("cesta/historico")]
    public async Task<ActionResult<List<CestaResponse>>> HistoricoAsync()
        => await SendCommand(new CestaHistoricoRequest());
}