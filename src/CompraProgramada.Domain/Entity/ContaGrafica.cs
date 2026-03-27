using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public class ContaGrafica : BaseConta
{
    public int ClienteId { get; init; }
    public string Tipo { get; } = "FILHOTE";
    public Cliente Cliente { get; private set; } = default!;
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
    public List<CustodiaFilhote> CustodiaFilhotes { get; init; } = new List<CustodiaFilhote>();
    public List<HistoricoCompra> HistoricoCompra { get; init; } = new List<HistoricoCompra>();

    private ContaGrafica(): base() { }

    internal ContaGrafica(int id, string numeroConta, DateTime dataCriacao, Cliente cliente, List<Distribuicao> distribuicoes, List<CustodiaFilhote> custodiaFilhotes, List<HistoricoCompra> historicoCompras)
        : base(id, numeroConta, dataCriacao)
    {
        ClienteId = cliente.Id;
        Cliente = cliente;
        Distribuicoes = distribuicoes;
        CustodiaFilhotes = custodiaFilhotes;
        HistoricoCompra = historicoCompras;
    }

    internal ContaGrafica(Cliente cliente)
        : base(0, cliente.Id)
    {
        ClienteId = cliente.Id;
        Cliente = cliente;
    }

    public static ContaGrafica Gerar(Cliente cliente)
        => new ContaGrafica(cliente);

    public void AdicionarDistribuicao(Distribuicao distribuicao)
        => Distribuicoes.Add(distribuicao);

    public void AdicionarCompra(HistoricoCompra compra)
        => HistoricoCompra.Add(compra);

    protected override string GerarNumeroConta(int id)
        => $"FLH-{id:D6}";
}