using System.Net;

namespace CompraProgramada.Shared.Exceptions.Base;

public class DomainException : Exception
{
    public string Erro { get; private set; } = string.Empty;
    public string Codigo { get; private set; } = string.Empty;
    public HttpStatusCode StatusCode { get; private set; }

    public DomainException(string message, string codigo, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        Erro = message;
        Codigo = codigo;
        StatusCode = statusCode;
    }
}