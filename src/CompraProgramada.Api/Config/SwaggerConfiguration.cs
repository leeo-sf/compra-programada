using Microsoft.OpenApi;

namespace CompraProgramada.Api.Config;

internal static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
        => services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Compra Programada",
                Version = "v1",
                Description = "API compra programada de ações"
            });

            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            foreach (var xmlFile in xmlFiles)
            {
                opt.IncludeXmlComments(xmlFile);
            }
        });

    public static void UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}