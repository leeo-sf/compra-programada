using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public class ContaMaster : BaseConta
{
    public string Tipo { get; } = "MASTER";
    public List<CustodiaMaster> CustodiaMasters { get; private set; } = new List<CustodiaMaster>();

    private ContaMaster(): base() { }

    internal ContaMaster(int id, string numeroConta, DateTime dataCriacao, List<CustodiaMaster> custodiaMasters)
        : base(id, numeroConta, dataCriacao) => CustodiaMasters = custodiaMasters;

    public void AtualizaCustodia(List<CustodiaMaster> custodias) => CustodiaMasters = custodias;
}