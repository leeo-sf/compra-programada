namespace CompraProgramada.Domain.Entity;

public class OrdemCompra
{
    public int Id { get; init; }
    public string Ticker { get; init; } = string.Empty;
    public int QuantidadeTotal { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal ValorTotal { get; init; }
    public DateTime Data { get; init; }
    public List<Distribuicao> Distribuicoes { get; private set; } = new List<Distribuicao>();
    public List<OrdemCompraDetalhe> Detalhes { get; private set; } = new List<OrdemCompraDetalhe>();

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

    public static OrdemCompra GerarOrdemCompra(string ticker, int quantidadeTotal, decimal precoUnitario, decimal valorTotal)
    {
        var detalhes = new List<OrdemCompraDetalhe>();
        var multiplosPresente = Math.DivRem(quantidadeTotal, 100, out int restos);

        if (restos != 0)
            detalhes.Add(OrdemCompraDetalhe.GerarDetalhes("FRACIONARIO", $"{ticker}F", restos, 0));

        if (multiplosPresente > 0)
            detalhes.Add(OrdemCompraDetalhe.GerarDetalhes("PADRAO", ticker, multiplosPresente * 100, 0));

        return new OrdemCompra(0, ticker, quantidadeTotal, precoUnitario, valorTotal, DateTime.Now, detalhes);
    }
}