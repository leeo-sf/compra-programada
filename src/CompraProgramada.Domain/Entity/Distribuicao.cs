namespace CompraProgramada.Domain.Entity;

public record Distribuicao(int Id, int OrdemCompraId, int ContaGraficaId, string Ticker, int QuantidadeAlocada, decimal ValorOperacao)
{
    public OrdemCompra OrdemCompra { get; init; } = default!;
    public ContaGrafica ContaGrafica { get; init; } = default!;
}