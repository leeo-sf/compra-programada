namespace CompraProgramada.Domain.Entity;

public class ComposicaoCesta
{
    public int Id { get; private set; }
    public int CestaId { get; private set; }
    public string Ticker { get; private set; } = default!;
    public decimal Percentual { get; private set; }
    public CestaRecomendada Cesta { get; init; } = default!;

    private ComposicaoCesta() { }

    internal ComposicaoCesta(int id, int cestaId, string ticker, decimal percentual, CestaRecomendada cesta)
    {
        Id = id;
        CestaId = cestaId;
        Ticker = ticker;
        Percentual = percentual;
        Cesta = cesta;
    }

    private ComposicaoCesta(int id, int cestaId, string ticker, decimal percentual)
    {
        Id = id;
        CestaId = cestaId;
        Ticker = ticker;
        Percentual = percentual;
    }

    public static ComposicaoCesta CriaItemNaCesta(string ticker, decimal percentual)
        => new ComposicaoCesta(0, 0, ticker.ToUpper(), percentual);
}