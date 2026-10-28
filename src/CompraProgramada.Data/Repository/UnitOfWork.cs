using CompraProgramada.Domain.Contract.Repository;

namespace CompraProgramada.Data.Repository;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task Salvar(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}