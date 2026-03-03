namespace CompraProgramada.Domain.Entity;

public record CustodiaFilhote(int Id, int ContaGraficaId, string? Ticker, int Quantidade = 0)
{
    public int Id { get; init; } = Id;
    public int ContaGraficaId { get; init; } = ContaGraficaId;
    public string? Ticker { get; init; } = Ticker;
    public int Quantidade { get; init; } = Quantidade;
    public ContaGrafica ContaGrafica { get; init; } = default!;
}