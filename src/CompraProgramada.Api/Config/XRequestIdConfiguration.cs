using System.Diagnostics.CodeAnalysis;

namespace CompraProgramada.Api.Config;

[ExcludeFromCodeCoverage]
internal static class XRequestIdConfiguration
{
    private const string HEADER_NAME = "X-Request-Id";

    public static void UseXRequestId(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey(HEADER_NAME))
                context.Request.Headers.Append(HEADER_NAME, Guid.NewGuid().ToString());

            await next();
        });
}