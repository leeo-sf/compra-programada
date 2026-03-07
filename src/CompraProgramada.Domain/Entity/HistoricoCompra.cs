namespace CompraProgramada.Domain.Entity;

public class HistoricoCompra
{
    public int Id { get; private set; }
    public int ContaGraficaId { get; private set; }
    public string Ticker { get; private set; } = default!;
    public int Quantidade { get; private set; }
    public decimal ValorAporte { get; private set; }
    public decimal PrecoExecutado { get; private set; }
    public decimal PrecoMedio { get; private set; }
    public DateOnly Data { get; private set; }
    public ContaGrafica ContaGrafica { get; set; } = default!;

    private HistoricoCompra() { }

    internal HistoricoCompra(int id, int contaGraficaId, string ticker, int quantidade, decimal precoExecutado, decimal precoMedio, decimal valorAporte, DateOnly data)
    {
        Id = id;
        ContaGraficaId = contaGraficaId;
        Ticker = ticker;
        Quantidade = quantidade;
        PrecoExecutado = precoExecutado;
        PrecoMedio = precoMedio;
        ValorAporte = valorAporte;
        Data = data;
    }

    public static HistoricoCompra RegistrarHistorico
        (int contaGraficaId, string ticker, int quantidade, decimal precoExecutado, decimal precoMedio, decimal valorAporte)
    {
        if (string.IsNullOrEmpty(ticker))
            throw new ApplicationException("O ativo precisa ser definido!");

        if (quantidade < 0)
            throw new ApplicationException("Quantidade não pode ser negativa");

        if (precoExecutado < 0)
            throw new ApplicationException("Preco de fechamento não pode ser negativo");

        return new HistoricoCompra(0, contaGraficaId, ticker, quantidade, precoExecutado, precoMedio, valorAporte, DateOnly.FromDateTime(DateTime.Now));
    }
}