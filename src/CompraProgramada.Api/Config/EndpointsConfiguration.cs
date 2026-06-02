using CompraProgramada.Api.Endpoints;

namespace CompraProgramada.Api.Config;

internal static class EndpointsConfiguration
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddAdministratorEndpoints();
        app.AddClienteEndpoints();
        app.AddMotorEndpoints();
    }
}