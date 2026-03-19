using CompraProgramada.Domain.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Domain.Exceptions;

public class QuantidadeNegativaException : DomainException
{
    public QuantidadeNegativaException()
        : base("Quantidade não pode ser negativa.",
            "QUANTIDADE_NEGATIVA",
            HttpStatusCode.UnprocessableEntity)
    { }
}