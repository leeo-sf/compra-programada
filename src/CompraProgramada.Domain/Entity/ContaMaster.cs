using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public class ContaMaster : BaseConta
{
    public string Tipo { get; } = "MASTER";
    public List<CustodiaMaster> CustodiaMasters { get; private set; } = new List<CustodiaMaster>();

    private ContaMaster(): base() { }

    internal ContaMaster(int id, string numeroConta, DateTime dataCriacao, List<CustodiaMaster> custodiaMasters)
        : base(id, numeroConta, dataCriacao) => CustodiaMasters = custodiaMasters;

    internal ContaMaster(int id, List<CustodiaMaster> custodiaMasters)
        : base(0, id) => CustodiaMasters = custodiaMasters;

    public static ContaMaster Gerar(int id, List<CustodiaMaster> custodias)
        => new ContaMaster(id, custodias);

    protected override string GerarNumeroConta(int id)
        => $"MST-{id:D6}";
}