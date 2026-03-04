namespace CompraProgramada.Application.Dto;

public class LoteCompraDto
{
    public required string Ticker { get; set; }
    public required int QuantidadeLotePadrao { get; set; }
    public required int QuantidadeLoteFracionario { get; set; }
}