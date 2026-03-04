namespace CompraProgramada.Application.Dto;

public class CustodiaFilhoteDto
{
    public required int Id { get; set; }
    public required int ContaGraficaId { get; set; }
    public required string Ticker { get; set; }
    public required int Quantidade { get; set; }
}