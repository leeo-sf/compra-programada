using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface ICustodiaFilhoteRepository
{
    Task<List<CustodiaFilhote>> AtualizarCustodiasAsync(List<CustodiaFilhote> custodias, CancellationToken cancellationToken);
}