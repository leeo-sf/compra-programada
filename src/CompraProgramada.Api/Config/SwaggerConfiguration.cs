using Microsoft.OpenApi;
using System.Diagnostics.CodeAnalysis;

namespace CompraProgramada.Api.Config;

[ExcludeFromCodeCoverage]
internal static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
        => services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Compra.Programada.Api",
                Version = "v1",
                Description = "API de compra programada de ações"
            });
        });

    public static void UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}