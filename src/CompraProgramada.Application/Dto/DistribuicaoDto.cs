namespace CompraProgramada.Application.Dto;

public class DistribuicaoDto
{
    public required int Id { get; init; }
    public int OrdemCompraId { get; init; }
    public required int ContaGraficaId { get; init; }
    public required string Ticker { get; init; }
    public required int QuantidadeAlocada { get; init; }
    public required decimal ValorOperacao { get; init; }
    public required ContaGraficaDto ContaGrafica { get; init; }
}