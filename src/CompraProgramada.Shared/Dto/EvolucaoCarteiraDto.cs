namespace CompraProgramada.Shared.Dto;

public class EvolucaoCarteiraDto
{
    public required DateOnly Data { get; set; }
    public required decimal ValorCarteira { get; set; }
    public required decimal ValorInvestido { get; set; }
    public required decimal Rentabilidade { get; set; }
}