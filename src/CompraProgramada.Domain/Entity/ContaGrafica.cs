using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public class ContaGrafica : BaseConta
{
    public int ClienteId { get; set; }
    public string Tipo { get; } = "FILHOTE";
    public Cliente Cliente { get; init; } = default!;
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
    public List<CustodiaFilhote> CustodiaFilhotes { get; init; } = new List<CustodiaFilhote>();
    public List<HistoricoCompra> HistoricoCompra { get; init; } = new List<HistoricoCompra>();

    private ContaGrafica(): base() { }

    internal ContaGrafica(int id, string numeroConta, DateTime dataCriacao, int clienteId, Cliente cliente, List<Distribuicao> distribuicoes, List<CustodiaFilhote> custodiaFilhotes, List<HistoricoCompra> historicoCompras)
        : base(id, numeroConta, dataCriacao)
    {
        ClienteId = clienteId;
        Cliente = cliente;
        Distribuicoes = distribuicoes;
        CustodiaFilhotes = custodiaFilhotes;
        HistoricoCompra = historicoCompras;
    }

    internal ContaGrafica(int id, string numeroConta, DateTime dataCriacao, int clienteId, List<CustodiaFilhote> custodias)
        : base(id, numeroConta, dataCriacao)
    {
        ClienteId = clienteId;
        CustodiaFilhotes = custodias;
    }

    public static ContaGrafica Gerar(int clienteId, List<CustodiaFilhote> custodias)
        => new ContaGrafica(0, GerarNumeroConta(clienteId, true), DateTime.Now, clienteId, custodias);
}