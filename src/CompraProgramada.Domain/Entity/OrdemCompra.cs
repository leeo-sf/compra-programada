namespace CompraProgramada.Domain.Entity;

public class OrdemCompra
{
    public int Id { get; protected set; }
    public string Ticker { get; protected set; } = string.Empty;
    public int QuantidadeTotal { get; protected set; }
    public decimal PrecoUnitario { get; protected set; }
    public decimal ValorTotal { get; protected set; }
    public DateTime Data { get; protected set; }
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
    public List<OrdemCompraDetalhe> Detalhes { get; init; } = new List<OrdemCompraDetalhe>();

    private OrdemCompra() { }

    internal OrdemCompra(int id, string ticker, int quantidadeTotal, decimal precoUnitario, decimal valorTotal, DateTime data)
    {
        Id = id;
        Ticker = ticker;
        QuantidadeTotal = quantidadeTotal;
        PrecoUnitario = precoUnitario;
        ValorTotal = valorTotal;
        Data = data;
    }

    internal OrdemCompra(int id, string ticker, int quantidadeTotal, decimal precoUnitario, decimal valorTotal, DateTime data, List<OrdemCompraDetalhe> detalhes)
    {
        Id = id;
        Ticker = ticker;
        QuantidadeTotal = quantidadeTotal;
        PrecoUnitario = precoUnitario;
        ValorTotal = valorTotal;
        Data = data;
        Detalhes = detalhes;
    }

    public static OrdemCompra GerarOrdemCompra(string ticker, int quantidadeTotal, decimal precoUnitario, decimal valorTotal, List<OrdemCompraDetalhe> detalhes)
        => new OrdemCompra(0, ticker, quantidadeTotal, precoUnitario, quantidadeTotal * precoUnitario, DateTime.Now, detalhes);
}