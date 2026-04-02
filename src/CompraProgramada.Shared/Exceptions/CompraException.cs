using CompraProgramada.Shared.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Shared.Exceptions;

public class CompraException : DomainException
{
    public CompraException(string mensagem, string codigo, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity)
        : base(mensagem, codigo, statusCode)
    { }
}