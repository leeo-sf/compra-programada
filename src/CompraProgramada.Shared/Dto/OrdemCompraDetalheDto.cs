namespace CompraProgramada.Shared.Dto;

public class OrdemCompraDetalheDto
{
    public required string Tipo { get; init; } = string.Empty;
    public required string Ticker { get; init; } = string.Empty;
    public required int Quantidade { get; init; }
}