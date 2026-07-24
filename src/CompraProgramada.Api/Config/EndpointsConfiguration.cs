using CompraProgramada.Api.Endpoints;

namespace CompraProgramada.Api.Config;

internal static class EndpointsConfiguration
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api");

        apiGroup.AddAdministratorEndpoints();
        apiGroup.AddClienteEndpoints();
        apiGroup.AddMotorEndpoints();
    }
}