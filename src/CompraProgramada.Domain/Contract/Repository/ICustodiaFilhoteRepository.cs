using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface ICustodiaFilhoteRepository
{
    Task<List<CustodiaFilhote>> AtualizarCustodiasAsync(List<CustodiaFilhote> custodias, CancellationToken cancellationToken);
}