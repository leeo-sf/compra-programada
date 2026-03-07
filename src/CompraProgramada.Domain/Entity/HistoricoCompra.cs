namespace CompraProgramada.Domain.Entity;

public record HistoricoCompra(int Id, DateOnly Data, decimal Valor, int ContaGraficaId)
{
    public ContaGrafica ContaGrafica { get; set; } = default!;
}