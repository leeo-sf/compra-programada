namespace CompraProgramada.Shared.Dto;

public class ResumoCarteiraDto
{
    public required decimal ValorTotalInvestido { get; init; }
    public required decimal ValorAtualCarteira { get; init; }
    public required decimal PlTotal { get; init; }
    public required decimal RentabilidadePercentual { get; init; }
}