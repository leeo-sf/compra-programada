namespace CompraProgramada.Domain.Entity.BaseEntity;

public abstract class BaseConta
{
    public int Id { get; init; }
    public string NumeroConta { get; init; } = string.Empty;
    public DateTime DataCriacao { get; init; }

    protected BaseConta() { }

    protected BaseConta(int id, string numeroConta, DateTime dataCriacao)
    {
        Id = id;
        NumeroConta = numeroConta;
        DataCriacao = dataCriacao;
    }

    protected BaseConta(int id, int correlationId)
    {
        Id = id;
        NumeroConta = GerarNumeroConta(correlationId);
        DataCriacao = DateTime.Now;
    }

    protected abstract string GerarNumeroConta(int id);
}