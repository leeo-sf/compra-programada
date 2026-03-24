using CompraProgramada.Domain.Enum;

namespace CompraProgramada.Domain.Entity;

public class OrdemCompraDetalhe
{
    public int Id { get; protected set; }
    public OrdemCompraTipo Tipo { get; protected set; } = default!;
    public string Ticker { get; protected set; } = default!;
    public int Quantidade { get; protected set; }
    public int OrdemCompraId { get; protected set; }
    public OrdemCompra OrdemCompra { get; protected set; } = default!;

    private OrdemCompraDetalhe() { }

    internal OrdemCompraDetalhe(int id, OrdemCompraTipo tipo, string ticker, int quantidade, int ordemCompraId, OrdemCompra ordemCompra)
    {
        Id = id;
        Ticker = ticker;
        Tipo = tipo;
        Quantidade = quantidade;
        OrdemCompraId = ordemCompraId;
        OrdemCompra = ordemCompra;
    }

    internal OrdemCompraDetalhe(int id, OrdemCompraTipo tipo, string ticker, int quantidade, int ordemCompraId)
    {
        Id = id;
        Ticker = ticker;
        Tipo = tipo;
        Quantidade = quantidade;
        OrdemCompraId = ordemCompraId;
    }

    public static OrdemCompraDetalhe GerarDetalhes(OrdemCompraTipo tipo, string ticker, int quantidade, int ordemCompraId)
        => new OrdemCompraDetalhe(0, tipo, ticker, quantidade, ordemCompraId);
}