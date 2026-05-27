using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IContaMasterRepository
{
    Task<ContaMaster> CriarAsync(ContaMaster conta, CancellationToken cancellationToken);
    Task<ContaMaster?> ObterContaMasterAsync(CancellationToken cancellationToken);
    Task<ContaMaster> AtualizarResiduosAysnc(ContaMaster conta, CancellationToken cancellationToken);
}