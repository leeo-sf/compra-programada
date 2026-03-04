namespace CompraProgramada.Application.Dto;

public class CustodiaMasterDto
{
    public required int Id { get; set; }
    public required int ContaMasterId { get; set; }
    public required string Ticker { get; set; }
    public required int QuantidadeResiduos { get; set; }
}