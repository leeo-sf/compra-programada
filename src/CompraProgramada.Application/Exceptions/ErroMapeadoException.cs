using CompraProgramada.Application.Response;
using System.Net;

namespace CompraProgramada.Application.Exceptions;

public class ErroMapeadoException : Exception
{
    public HttpStatusCode StatusCode { get; init; }
    public ErroResponse ErroDetalhes { get; init; }

    public ErroMapeadoException(string message, string codigo, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        ErroDetalhes = new ErroResponse { Erro = message, Codigo = codigo };
        StatusCode = httpStatusCode;
    }
}