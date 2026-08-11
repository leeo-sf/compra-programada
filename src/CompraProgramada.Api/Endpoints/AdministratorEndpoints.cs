using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;

namespace CompraProgramada.Api.Endpoints;

internal static class AdministratorEndpoints
{
    public static void AddAdministratorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin")
            .WithTags("Administrador");

        group.MapPost("cesta", CriarCestaAsync)
            .Produces<CriarCestaRecomendadaResponse>(StatusCodes.Status201Created)
            .WithSummary("Criar cesta recomendada de investimento");

        group.MapGet("cesta/atual", ObterCestaAtualAsync)
            .Produces<CestaRecomendadaDto>(StatusCodes.Status200OK)
            .WithSummary("Obtem detalhes da cesta atual");

        group.MapGet("cesta/historico", ObterHistoricoCestasAsync)
            .Produces<HistoricoCestasResponse>(StatusCodes.Status200OK)
            .WithSummary("Obtem o histórico de cestas recomendadas");

        group.MapGet("conta-master/custodia", ConsultarCustodiaMaster)
            .Produces<ConsultarCustodiaMasterResponse>(StatusCodes.Status200OK)
            .WithSummary("Obtem informações sobre a custódia dos ativos da conta master");
    }

    private static async Task<IResult> CriarCestaAsync(IMediator mediator, CriarCestaRecomendadaRequest request)
        => await mediator.SendCommand(request, 201);

    private static async Task<IResult> ObterCestaAtualAsync(IMediator mediator)
        => await mediator.SendCommand(new CestaAtualRequest());

    private static async Task<IResult> ObterHistoricoCestasAsync(IMediator mediator)
        => await mediator.SendCommand(new CestaHistoricoRequest());

    private static async Task<IResult> ConsultarCustodiaMaster(IMediator mediator)
        => await mediator.SendCommand(new ConsultarCustodiaMasterRequest());
}