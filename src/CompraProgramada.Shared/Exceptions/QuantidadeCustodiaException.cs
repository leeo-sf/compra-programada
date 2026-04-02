using CompraProgramada.Shared.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Shared.Exceptions;

public class QuantidadeCustodiaException : DomainException
{
    public QuantidadeCustodiaException()
        : base("Quantidade deve ser maior que zero.",
            "QUANTIDADE_INVALIDA",
            HttpStatusCode.UnprocessableEntity)
    { }
}