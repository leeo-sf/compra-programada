namespace CompraProgramada.Domain.Entity;

public class CustodiaMaster
{
    public int Id { get; set; }
    public int ContaMasterId { get; set; }
    public string Ticker { get; set; } = default!;
    public int QuantidadeResiduo { get; set; }
    public ContaMaster ContaMaster { get; init; } = default!;
}