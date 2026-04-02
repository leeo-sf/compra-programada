namespace CompraProgramada.Domain.Entity;

public class ComposicaoCotacao
{
    public int Id { get; init; }
    public int CotacaoId { get; init; }
    public string Ticker { get; init; } = string.Empty;
    public decimal PrecoFechamento { get; init; }
    public Cotacao Cotacao { get; init; } = default!;

    private ComposicaoCotacao() { }

    internal ComposicaoCotacao(int id, int cotacaoId, string ticker, decimal precoFechamento)
    {
        Id = id;
        CotacaoId = cotacaoId;
        Ticker = ticker;
        PrecoFechamento = precoFechamento;
    }

    public static ComposicaoCotacao CriarItem(string ticker, decimal precoFechamento)
        => new(0, 0, ticker, precoFechamento);
}