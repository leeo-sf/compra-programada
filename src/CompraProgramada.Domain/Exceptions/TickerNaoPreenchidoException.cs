using CompraProgramada.Domain.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Domain.Exceptions;

public class TickerNaoPreenchidoException : DomainException
{
    public TickerNaoPreenchidoException()
        : base("O nome do ativo não pode estar em branco.",
            "TICKER_INVALIDO",
            HttpStatusCode.UnprocessableEntity)
    { }
}