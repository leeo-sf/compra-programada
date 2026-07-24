using CompraProgramada.Shared.Exceptions.Base;
using System.Net;

namespace CompraProgramada.Shared.Exceptions;

public class TickerNaoPreenchidoException : DomainException
{
    public TickerNaoPreenchidoException()
        : base("O nome do ativo deve ser preenchido",
            "TICKER_INVALIDO",
            HttpStatusCode.UnprocessableEntity)
    { }
}