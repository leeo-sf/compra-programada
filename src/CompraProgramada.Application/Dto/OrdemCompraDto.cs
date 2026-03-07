namespace CompraProgramada.Application.Dto;

public class OrdemCompraDto
{
    public int Id { get; set; }
    public required string Ticker { get; init; }
    public required int QuantidadeCompra { get; init; }
    public required decimal PrecoExecucao { get; init; }
    public required DateTime Data { get; init; }
}