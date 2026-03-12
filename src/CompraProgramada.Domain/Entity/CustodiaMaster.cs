namespace CompraProgramada.Domain.Entity;

public class CustodiaMaster
{
    public int Id { get; init; }
    public int ContaMasterId { get; init; }
    public string Ticker { get; init; } = default!;
    public int QuantidadeResiduo { get; private set; }
    public ContaMaster ContaMaster { get; init; } = default!;

    private CustodiaMaster() { }

    internal CustodiaMaster(int id, int contaMasterId, string ticker, int quantidadeResiduo, ContaMaster contaMaster)
    {
        Id = id;
        ContaMasterId = contaMasterId;
        Ticker = ticker;
        QuantidadeResiduo = quantidadeResiduo;
        ContaMaster = contaMaster;
    }

    internal CustodiaMaster(int id, int contaMasterId, string ticker, int quantidadeResiduo)
    {
        Id = id;
        ContaMasterId = contaMasterId;
        Ticker = ticker;
        QuantidadeResiduo = quantidadeResiduo;
    }

    public static CustodiaMaster CriarCustodia(int contaMasterId, string ticker, int quantidadeResiduo)
        => new(0, contaMasterId, ticker, quantidadeResiduo);

    public void AtualizarResiduo(int novaQuantidade)
        => QuantidadeResiduo = novaQuantidade;
}