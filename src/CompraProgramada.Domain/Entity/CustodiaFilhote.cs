namespace CompraProgramada.Domain.Entity;

public record CustodiaFilhote(int Id, int ContaGraficaId, string? Ticker, int Quantidade = 0)
{
    public ContaGrafica ContaGrafica { get; init; } = default!;
}