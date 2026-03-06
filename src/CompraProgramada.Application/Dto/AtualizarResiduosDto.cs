namespace CompraProgramada.Application.Dto;

public class AtualizarResiduosDto
{
    public required int CustodiaId { get; init; }
    public required string Ticker { get; init; }
    public required int QuantidadeCompra { get; init; }
    public required decimal ValorCompra { get; init; }
    public required decimal PrecoFechamento { get; init; }
}