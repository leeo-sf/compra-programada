using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IOrdemCompraService
{
    Task<Result<List<OrdemCompra>>> EmitirOrdensDeCompraAsync(decimal valorTotalConsolidado, CancellationToken cancellationToken);
}