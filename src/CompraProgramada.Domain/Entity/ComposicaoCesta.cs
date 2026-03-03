namespace CompraProgramada.Domain.Entity;

public record ComposicaoCesta(int Id, int CestaId, string Ticker, decimal Percentual)
{
    public CestaRecomendada Cesta { get; init; } = default!;
}