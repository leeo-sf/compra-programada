using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompraProgramada.Api.Controllers;

[Route("api/admin")]
public class AdministradorController(IMediator mediator) : BaseController(mediator)
{
    /// <summary>
    /// Criar cesta de investimento
    /// </summary>
    [HttpPost("cesta")]
    public async Task<ActionResult<CriarCestaRecomendadaResponse>> CestaAsync(CriarCestaRecomendadaRequest request)
        => await SendCommand(request, 201);

    /// <summary>
    /// Consultar cesta atual
    /// </summary>
    [HttpGet("cesta/atual")]
    public async Task<ActionResult<CestaRecomendadaResponse>> CestaAtualAsync()
        => await SendCommand(new CestaAtualRequest());

    /// <summary>
    /// Consultar histórico de cestas
    /// </summary>
    [HttpGet("cesta/historico")]
    public async Task<ActionResult<List<CestaRecomendadaResponse>>> HistoricoAsync()
        => await SendCommand(new CestaHistoricoRequest());
}