namespace CompraProgramada.Domain.Entity;

public record CustodiaMaster(int Id, int ContaMasterId, string Ticker, int QuantidadeResiduo, bool ConsideradoNovaCompra = false)
{
    public ContaMaster ContaMaster { get; init; } = default!;
}