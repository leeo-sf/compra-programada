using CompraProgramada.Domain.Entity.BaseEntity;
using CompraProgramada.Shared.Exceptions;

namespace CompraProgramada.Domain.Entity;

public class CustodiaFilhote : EntidadeBase
{
    public int ContaGraficaId { get; init; }
    public string Ticker { get; init; } = string.Empty;
    public decimal PrecoMedio { get; private set; }
    public int Quantidade { get; private set; }
    public ContaGrafica ContaGrafica { get; init; } = default!;
    public decimal ValorInvestido => Quantidade * PrecoMedio;

    internal CustodiaFilhote(int contaGraficaId, string ticker, decimal precoMedio, int quantidade)
        : base(0)
    {
        if (string.IsNullOrEmpty(ticker))
            throw new TickerNaoPreenchidoException();

        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        PrecoMedio = precoMedio;
        Quantidade = quantidade;
    }

    public static CustodiaFilhote GerarCustodia(int contaGraficaId, string ticker)
        => new(contaGraficaId, ticker, 0, 0);

    /// <summary>
    /// Atribuí nova quantidade de ativos (soma quantidade anterior + nova quantidade)
    /// </summary>
    /// <param name="novaQuantidade">Nova quantidade a ser atribuída</param>
    public void AdicionarNovaQuantidade(int novaQuantidade)
    {
        if (novaQuantidade < 0)
            throw new QuantidadeNegativaException();

        Quantidade += novaQuantidade;
    }

    /// <summary>
    /// Calcula e atualiza preço médio ponderado de compra do ativo
    /// </summary>
    /// <param name="precoUnitario">Preço unitário do ativo</param>
    /// <param name="novaQuantidadeDeAtivos">Nova quantidade de ativos que serão atribuídos a conta do cliente</param>
    /// <returns>Valor do preço médio</returns>
    public decimal CalcularPrecoMedio(decimal precoUnitario, int novaQuantidadeDeAtivos)
    {
        if (novaQuantidadeDeAtivos < 1)
            return 0;

        var precoMedioCalculado = (Quantidade * PrecoMedio + novaQuantidadeDeAtivos * precoUnitario) / (Quantidade + novaQuantidadeDeAtivos);
        PrecoMedio = Math.Round(precoMedioCalculado, 2); 

        return PrecoMedio;
    }

    /// <summary>
    /// Calcula valor de PL (Lucro/Prejuízo) por ativo
    /// </summary>
    /// <param name="precoFechamento">Valor de fechamento do ativo</param>
    /// <returns>Valor da PL</returns>
    internal decimal CalcularPl(decimal precoFechamento)
    {
        if (Quantidade < 0)
            throw new QuantidadeCustodiaException();

        return (precoFechamento - PrecoMedio) * Quantidade;
    }

    /// <summary>
    /// Calcula valor atual da carteira
    /// </summary>
    /// <param name="precoFechamento">Valor de fechamento do ativo</param>
    /// <returns>Valor atual da carteira</returns>
    internal decimal CalcularValorAtualCarteira(decimal precoFechamento)
    {
        var valorAtual = Quantidade * precoFechamento;

        return Math.Round(valorAtual, 2);
    }
}