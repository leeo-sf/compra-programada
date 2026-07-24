using CompraProgramada.Shared.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Shared.Exceptions;

public class AppException : DomainException
{
    public AppException(string mensagem, string codigo, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity)
        : base(mensagem, codigo, statusCode)
    { }
}