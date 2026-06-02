using System.Diagnostics.CodeAnalysis;

namespace CompraProgramada.Api.Config;

[ExcludeFromCodeCoverage]
internal static class XRequestIdConfiguration
{
    public static void UseXRequestId(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey("X-Request-Id"))
                context.Request.Headers.Append("X-Request-Id", Guid.NewGuid().ToString());

            await next();
        });
}