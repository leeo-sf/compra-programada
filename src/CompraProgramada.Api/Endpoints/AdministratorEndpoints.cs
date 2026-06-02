using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;

namespace CompraProgramada.Api.Endpoints;

internal static class AdministratorEndpoints
{
    public static void AddAdministratorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin/cesta")
            .WithTags("Administrador");

        group.MapPost(string.Empty, CriarCestaAsync)
            .Produces(StatusCodes.Status201Created)
            .WithSummary("Criar cesta recomendada de investimento");

        group.MapGet("atual", ObterCestaAtualAsync)
            .Produces<CestaRecomendadaDto>(StatusCodes.Status200OK)
            .WithSummary("Obtem detalhes da cesta atual");

        group.MapGet("historico", ObterHistoricoCestasAsync)
            .Produces<HistoricoCestasResponse>(StatusCodes.Status200OK)
            .WithSummary("Obtem o histórico de cestas recomendadas");
    }

    private static async Task<IResult> CriarCestaAsync(IMediator mediator, CriarCestaRecomendadaRequest request)
        => await mediator.SendCommand(request, 201);

    private static async Task<IResult> ObterCestaAtualAsync(IMediator mediator)
        => await mediator.SendCommand(new CestaAtualRequest());

    private static async Task<IResult> ObterHistoricoCestasAsync(IMediator mediator)
        => await mediator.SendCommand(new CestaHistoricoRequest());
}