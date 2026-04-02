using CompraProgramada.Shared.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Shared.Exceptions;

public class TickerNaoPreenchidoException : DomainException
{
    public TickerNaoPreenchidoException()
        : base("O nome do ativo não pode estar em branco.",
            "TICKER_INVALIDO",
            HttpStatusCode.UnprocessableEntity)
    { }
}