namespace CompraProgramada.Domain.Entity;

public record CustodiaMaster(int Id, int ContaMasterId, string Ticker, int QuantidadeResiduo)
{
    public ContaMaster ContaMaster { get; init; } = default!;
}