using CompraProgramada.Domain.Exceptions;

namespace CompraProgramada.Domain.Entity;

public class Distribuicao
{
    public int Id { get; init; }
    public int OrdemCompraId { get; init; }
    public int ContaGraficaId { get; init; }
    public string Ticker { get; init; } = string.Empty;
    public int QuantidadeAlocada { get; init; }
    public decimal ValorOperacao { get; init; }
    public OrdemCompra OrdemCompra { get; init; } = default!;
    public ContaGrafica ContaGrafica { get; init; } = default!;

    private Distribuicao() { }

    internal Distribuicao(int id, int ordemCompraId, int contaGraficaId, string ticker, int quantidadeAlocada, decimal valorOperacao, OrdemCompra ordemCompra, ContaGrafica contaGrafica)
    {
        if (string.IsNullOrEmpty(ticker))
            throw new TickerNaoPreenchidoException();

        if (quantidadeAlocada < 0)
            throw new QuantidadeNegativaException();

        Id = id;
        OrdemCompraId = ordemCompraId;
        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        QuantidadeAlocada = quantidadeAlocada;
        ValorOperacao = valorOperacao;
        OrdemCompra = ordemCompra;
        ContaGrafica = contaGrafica;
    }

    public static Distribuicao CriarDistribuicao(int quantidadeAlocada, ContaGrafica conta, OrdemCompra ordemCompra)
        => new Distribuicao(
            0,
            ordemCompra.Id,
            conta.Id,
            ordemCompra.Ticker,
            quantidadeAlocada,
            quantidadeAlocada * ordemCompra.PrecoUnitario,
            ordemCompra,
            conta);
}