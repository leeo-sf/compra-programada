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

    public void AdicionarNovaQuantidade(int novaQuantidade)
    {
        if (novaQuantidade < 0)
            throw new QuantidadeNegativaException();

        Quantidade += novaQuantidade;
    }

    /// <summary>
    /// Calcula e atualiza o valor do preço médio
    /// </summary>
    /// <param name="precoFechamentoAtivo">Valor do fechamento do ativo</param>
    /// <param name="novaQuantidadeDeAtivos">Nova quantidade de ativos que serão atribuídos a conta do cliente</param>
    /// <returns>Valor do preço médio</returns>
    public decimal CalcularPrecoMedio(decimal precoFechamentoAtivo, int novaQuantidadeDeAtivos)
    {
        if (novaQuantidadeDeAtivos == 0)
            return 0;

        var valorCompraAnterior = Quantidade * PrecoMedio;
        var valorCompraAtual = novaQuantidadeDeAtivos * precoFechamentoAtivo;

        decimal precoMedio = valorCompraAtual / novaQuantidadeDeAtivos;

        if (Quantidade > 0)
            precoMedio = (valorCompraAnterior + valorCompraAtual) / Quantidade + novaQuantidadeDeAtivos;

        PrecoMedio = precoMedio;
        return precoMedio;
    }
}