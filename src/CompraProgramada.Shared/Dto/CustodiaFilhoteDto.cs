namespace CompraProgramada.Shared.Dto;

public class CustodiaFilhoteDto
{
    public required int Id { get; init; }
    public required int ContaGraficaId { get; init; }
    public required string Ticker { get; init; }
    public required decimal PrecoMedio { get; init; }
    public required int Quantidade { get; init; }
}