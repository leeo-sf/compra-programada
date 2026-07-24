using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;

namespace CompraProgramada.Api.Endpoints;

internal static class ClientesEndpoints
{
    public static void AddClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/clientes")
            .WithTags("Clientes");

        group.MapPost("adesao", RealizarAdesaoAsync)
            .Produces<AdesaoResponse>(StatusCodes.Status201Created)
            .WithSummary("Realiza adesão do produto")
            .WithDescription("Adere ao produto e sempre que o sistema realizar compras será distribuído ativos à sua conta");

        group.MapPost("{id}/saida", SairDoProdutoAsync)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Cancela a permanência do cliente ao produto");

        group.MapPut("{id}/valor-mensal", AtualizarValorMensalAsync)
            .Produces<AtualizarValorMensalResponse>(StatusCodes.Status200OK)
            .WithSummary("Atualiza o valor mensal do cliente à ser investido");

        group.MapGet("{id}/carteira", ConsultarCarteiraAsync)
            .Produces<CarteiraCustodiaResponse>(StatusCodes.Status200OK)
            .WithSummary("Consulta detalhes da carteira");

        group.MapGet("{id}/rentabilidade", ConsultarRentabilidadeAsync)
            .Produces<RentabilidadeResponse>(StatusCodes.Status200OK)
            .WithSummary("Consulta detalhes da rentabilidade da carteira");
    }

    private static async Task<IResult> RealizarAdesaoAsync(IMediator mediator, AdesaoRequest request)
        => await mediator.SendCommand(request, 201);

    private static async Task<IResult> SairDoProdutoAsync(IMediator mediator, int id)
        => await mediator.SendCommand(new SaidaProdutoRequest(id));

    private static async Task<IResult> AtualizarValorMensalAsync(IMediator mediator, int id, AtualizarValorMensalRequest request)
        => await mediator.SendCommand(request with { ClienteId = id });

    private static async Task<IResult> ConsultarCarteiraAsync(IMediator mediator, int id)
        => await mediator.SendCommand(new CarteiraCustodiaRequest(id));

    private static async Task<IResult> ConsultarRentabilidadeAsync(IMediator mediator, int id)
        => await mediator.SendCommand(new RentabilidadeRequest(id));
}