namespace CompraProgramada.Domain.Entity.BaseEntity;

public abstract class EntidadeBase
{
    public int Id { get; init; }
    public DateTime DataCriacao { get; init; }
    public DateTime DataAtualizacao { get; protected set; }

    protected EntidadeBase() { }

    protected EntidadeBase(int id, DateTime dataCriacao, DateTime dataAtualizacao)
    {
        Id = id;
        DataCriacao = dataCriacao;
        DataAtualizacao = dataAtualizacao;
    }

    protected EntidadeBase(int id)
    {
        Id = id;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }
}