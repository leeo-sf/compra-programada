using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IOrdemCompraRepository
{
    Task<List<OrdemCompra>> SalvarOrdensDeCompra(List<OrdemCompra> ordemCompra, CancellationToken cancellationToken);
}