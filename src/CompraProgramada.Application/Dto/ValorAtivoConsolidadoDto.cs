namespace CompraProgramada.Application.Dto;

public class ValorAtivoConsolidadoDto
{
    public required string Ticker { get; init; }
    public required decimal ValorDeCompraPorAtivo { get; init; }
}