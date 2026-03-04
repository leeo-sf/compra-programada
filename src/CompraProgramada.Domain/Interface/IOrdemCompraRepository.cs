using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IOrdemCompraRepository
{
    Task SalvarOrdemDeCompra(OrdemCompra ordemCompra, CancellationToken cancellationToken);
}