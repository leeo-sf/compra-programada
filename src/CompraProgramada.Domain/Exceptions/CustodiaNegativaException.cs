using CompraProgramada.Domain.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Domain.Exceptions;

public class CustodiaNegativaException : DomainException
{
    public CustodiaNegativaException()
        : base("Quantidade de custodia não pode ser negativa.",
            "QUANTIDADE_CUSTODIA_INVALIDA",
            HttpStatusCode.UnprocessableEntity)
    { }
}