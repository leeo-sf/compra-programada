using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
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
    public async Task<ActionResult<CestaRecomendadaDto>> CestaAtualAsync()
        => await SendCommand(new CestaAtualRequest());

    /// <summary>
    /// Consultar histórico de cestas
    /// </summary>
    [HttpGet("cesta/historico")]
    public async Task<ActionResult<HistoricoCestasResponse>> HistoricoAsync()
        => await SendCommand(new CestaHistoricoRequest());

    /// <summary>
    /// Consultar conta master e resíduos de ativos
    /// </summary>
    [HttpGet("conta-master/custodia")]
    public async Task<ActionResult<ContaMasterCustodiaResponse>> ContaMasterCustodiaAsync()
        => await SendCommand(new ContaMasterCustodiaRequest());
}