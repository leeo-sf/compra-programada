namespace CompraProgramada.Application.Dto;

public record GrupoAtivoCompraDto
{
    public required string Ticker { get; init; }
    public required int Quantidade { get; init; }
    public required decimal PrecoFechamento { get; init; }
}