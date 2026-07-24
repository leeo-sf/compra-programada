using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;

namespace CompraProgramada.Api.Endpoints;

internal static class MotorEndpoints
{
    public static void AddMotorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/motor")
            .WithTags("Motor");

        group.MapPost("executar-compra", ExecutarCompraAsync)
            .Produces<ExecutarMotorCompraResponse>(StatusCodes.Status200OK)
            .WithSummary("Executa o motor de compra")
            .WithDescription("Endpoint disponível para realização de testes");
    }

    private static async Task<IResult> ExecutarCompraAsync(IMediator mediator, ExecutarMotorCompraRequest request)
        => await mediator.SendCommand(request);
}