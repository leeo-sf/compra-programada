using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IContaMasterRepository
{
    Task<ContaMaster?> ObterContaMasterAsync(CancellationToken cancellationToken);
}