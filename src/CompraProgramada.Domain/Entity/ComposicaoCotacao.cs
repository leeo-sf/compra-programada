namespace CompraProgramada.Domain.Entity;

public record ComposicaoCotacao(int Id, int CotacaoId, string Ticker, decimal PrecoFechamento)
{
    public Cotacao Cotacao { get; init; } = default!;
}