namespace CompraProgramada.Domain.Entity;

public record EvolucaoCarteira(int Id, DateOnly Data, decimal ValorCarteira, decimal ValorInvestido, decimal Rentabilidade, int ContaGraficaId)
{
    public ContaGrafica ContaGrafica { get; set; } = default!;
}