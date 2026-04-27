namespace CompraProgramada.Api;

internal class RequestIdMiddleware
{
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers["X-Request-Id"].FirstOrDefault() ??
            Guid.NewGuid().ToString();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Request-Id"] = requestId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}