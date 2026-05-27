using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IOrdemCompraRepository
{
    Task<List<OrdemCompra>?> ObterOrdensCompraAsync(DateTime data, CancellationToken cancellationToken);
    Task<List<OrdemCompra>> SalvarOrdensDeCompra(List<OrdemCompra> ordemCompra, CancellationToken cancellationToken);
}