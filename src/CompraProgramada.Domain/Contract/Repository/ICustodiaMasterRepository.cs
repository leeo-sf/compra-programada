using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface ICustodiaMasterRepository
{
    void Criar(List<CustodiaMaster> custodias);
    Task<List<CustodiaMaster>?> ObterResiduosAsync(CancellationToken cancellationToken);
    Task AtualizarResiduosAysnc(List<CustodiaMaster> custodias, CancellationToken cancellationToken);
}