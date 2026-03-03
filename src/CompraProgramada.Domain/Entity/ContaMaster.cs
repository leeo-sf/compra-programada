using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public record ContaMaster(int Id, string NumeroConta, DateTime DataCriacao)
    : BaseConta(Id, NumeroConta, DataCriacao)
{
    public string Tipo { get; } = "MASTER";
}