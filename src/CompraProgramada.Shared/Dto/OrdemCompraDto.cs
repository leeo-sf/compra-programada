namespace CompraProgramada.Shared.Dto;

public class OrdemCompraDto
{
    public required string Ticker { get; init; } = string.Empty;
    public required int QuantidadeTotal { get; init; }
    public required List<OrdemCompraDetalheDto> Detalhes { get; init; }
    public required decimal PrecoUnitario { get; init; }
    public decimal ValorTotal => QuantidadeTotal * PrecoUnitario;
}