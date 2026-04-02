namespace CompraProgramada.Shared.Dto;

public class HistoricoCompraDto
{
    public required int ContaGraficaId { get; init; }
    public required string Ticker { get; init; }
    public required int Quantidade { get; init; }
    public required decimal ValorAporte { get; init; }
    public required decimal PrecoExecutado { get; init; }
    public required decimal PrecoMedio { get; init; }
    public required DateOnly Data { get; init; }
}