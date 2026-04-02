using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface IOrdemCompraService
{
    Task<Result<List<OrdemCompra>>> EmitirOrdensDeCompraAsync(decimal valorTotalConsolidado, CancellationToken cancellationToken);
}