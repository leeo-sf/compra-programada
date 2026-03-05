namespace CompraProgramada.Application.Dto;

public class ResumoCarteiraDto
{
    public required decimal ValorTotalInvestido { get; set; }
    public required decimal ValorAtualCarteira { get; set; }
    public required decimal PlTotal { get; set; }
    public required decimal RentabilidadePercentual { get; set; }
}