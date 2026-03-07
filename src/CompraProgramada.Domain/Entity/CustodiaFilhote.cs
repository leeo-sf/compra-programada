namespace CompraProgramada.Domain.Entity;

public record CustodiaFilhote(int Id, int ContaGraficaId, string Ticker, decimal PrecoMedio = 0, int Quantidade = 0)
{
    public ContaGrafica ContaGrafica { get; init; } = default!;
}