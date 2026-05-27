using CompraProgramada.Shared.Response;
using OperationResult;

namespace CompraProgramada.Domain.Contract.Service;

public interface ICompraService
{
    Task<Result<ExecutarCompraResponse?>> ExecutarCompraAsync(DateTime? data, CancellationToken cancellationToken);
}