namespace CompraProgramada.Domain.Contract.Repository;

public interface IUnitOfWork
{
    Task Salvar(CancellationToken cancellationToken);
}