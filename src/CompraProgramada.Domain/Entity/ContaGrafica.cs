using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public record ContaGrafica(int Id, string NumeroConta, DateTime DataCriacao, int ClienteId)
    : BaseConta(Id, NumeroConta, DataCriacao)
{
    public string Tipo { get; } = "FILHOTE";
    public Cliente Cliente { get; init; } = default!;
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
    public List<CustodiaFilhote> CustodiaFilhotes { get; init; } = new List<CustodiaFilhote>();
}