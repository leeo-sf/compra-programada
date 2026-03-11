using CompraProgramada.Domain.Exceptions.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Infra.Middleware;

internal class DomainExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DomainExceptionHandler> _logger;

    public DomainExceptionHandler(ILogger<DomainExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is DomainException ex)
        {
            _logger.LogError("Ocorreu uma exceção mapeada: {TipoExcecao} --> {Mensagem}", ex.GetType().Name,  exception.Message);

            var response = new
            {
                Mensagem = ex.Message,
                Codigo = ex.Codigo
            };

            httpContext.Response.StatusCode = (int)ex.StatusCode;

            await httpContext.Response.WriteAsJsonAsync(response);

            return true;
        }

        return false;
    }
}