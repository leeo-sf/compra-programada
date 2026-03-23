using CompraProgramada.Domain.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Domain.Exceptions;

public class CompraException : DomainException
{
    public CompraException(string mensagem, string codigo, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity)
        : base(mensagem, codigo, statusCode)
    { }
}