using CompraProgramada.Domain.Exceptions;

namespace CompraProgramada.Domain.Entity;

public class CustodiaFilhote
{
    public int Id { get; private set; }
    public int ContaGraficaId { get; private set; }
    public string Ticker { get; private set; } = string.Empty;
    public decimal PrecoMedio { get; private set; }
    public int Quantidade { get; private set; }
    public ContaGrafica ContaGrafica { get; init; } = default!;

    private CustodiaFilhote() { }

    internal CustodiaFilhote(int id, int contaGraficaId, string ticker, decimal precoMedio, int quantidade, ContaGrafica contaGrafica)
    {
        Id = id;
        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        PrecoMedio = precoMedio;
        Quantidade = quantidade;
        ContaGrafica = contaGrafica;
    }

    internal CustodiaFilhote(int id, int contaGraficaId, string ticker, decimal precoMedio, int quantidade)
    {
        if (string.IsNullOrEmpty(ticker))
            throw new TickerNaoPreenchidoException();

        Id = id;
        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        PrecoMedio = precoMedio;
        Quantidade = quantidade;
    }

    public static CustodiaFilhote GerarCustodia(string ticker)
        => new CustodiaFilhote(0, 0, ticker, 0, 0);

    public void Atualizar(decimal precoMedio, int novaQuantidade)
    {
        if (novaQuantidade < 0)
            throw new QuantidadeNegativaException();

        PrecoMedio = precoMedio;
        Quantidade = novaQuantidade;
    }
}