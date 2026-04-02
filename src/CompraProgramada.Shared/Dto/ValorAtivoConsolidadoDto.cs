namespace CompraProgramada.Shared.Dto;

public class ValorAtivoConsolidadoDto
{
    public required string Ticker { get; init; } = string.Empty;
    public required decimal ValorDeCompraAtivo { get; init; }
}