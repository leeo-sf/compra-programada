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
        Id = id;
        OrdemCompraId = ordemCompraId;
        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        QuantidadeAlocada = quantidadeAlocada;
        ValorOperacao = valorOperacao;
        OrdemCompra = ordemCompra;
        ContaGrafica = contaGrafica;
    }

    internal Distribuicao(int id, int ordemCompraId, int contaGraficaId, string ticker, int quantidadeAlocada, decimal valorOperacao)
    {
        Id = id;
        OrdemCompraId = ordemCompraId;
        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        QuantidadeAlocada = quantidadeAlocada;
        ValorOperacao = valorOperacao;
    }

    public static Distribuicao CriarDistribuicao(int ordemCompraId, int contaGraficaId, string ticker, int quantidadeAlocada, decimal valorOperacao)
        => new Distribuicao(0, ordemCompraId, contaGraficaId, ticker, quantidadeAlocada, valorOperacao);
}