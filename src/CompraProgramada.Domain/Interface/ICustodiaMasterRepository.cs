using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface ICustodiaMasterRepository
{
    Task<List<CustodiaMaster>> CriarAsync(List<CustodiaMaster> custodias, CancellationToken cancellationToken);
    Task<List<CustodiaMaster>?> ObterResiduosAsync(CancellationToken cancellationToken);
    Task AtualizarResiduosAysnc(List<CustodiaMaster> conta, CancellationToken cancellationToken);
}