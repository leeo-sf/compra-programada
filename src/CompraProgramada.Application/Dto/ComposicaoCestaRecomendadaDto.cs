namespace CompraProgramada.Application.Dto;

public class ComposicaoCestaRecomendadaDto
{
    public required int Id { get; set; }
    public required int CestaId { get; set; }
    public required string Ticker { get; set; }
    public required decimal Percentual { get; set; }
}