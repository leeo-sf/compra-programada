namespace CompraProgramada.Domain.Entity;

public class ComposicaoCesta
{
    public int Id { get; private set; }
    public int CestaId { get; private set; }
    public string Ticker { get; private set; } = default!;
    public decimal Percentual { get; private set; }
    public CestaRecomendada Cesta { get; init; } = default!;

    private ComposicaoCesta() { }

    private ComposicaoCesta(int id, int cestaId, string ticker, decimal percentual)
    {
        Id = id;
        CestaId = cestaId;
        Ticker = ticker;
        Percentual = percentual;
    }

    public static ComposicaoCesta CriaItemNaCesta(string ticker, decimal percentual)
        => new ComposicaoCesta(0, 0, ticker.ToUpper(), percentual);

    public decimal ValorConsolidado(decimal valorTotalConsolidado)
        => valorTotalConsolidado * (Percentual / 100);
}