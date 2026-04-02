using CompraProgramada.Shared.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Shared.Exceptions;

public class ClienteNaoEncontradoException : DomainException
{
    public ClienteNaoEncontradoException()
        : base("Cliente nao encontrado.",
            "CLIENTE_NAO_ENCONTRADO",
            HttpStatusCode.NotFound)
    { }
}