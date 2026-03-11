using CompraProgramada.Domain.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Domain.Exceptions;

public class ClienteNaoEncontradoException : DomainException
{
    public ClienteNaoEncontradoException()
        : base("Cliente nao encontrado.",
            "CLIENTE_NAO_ENCONTRADO",
            HttpStatusCode.NotFound)
    { }
}