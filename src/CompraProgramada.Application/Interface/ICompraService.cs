using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Response;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICompraService
{
    Task<Result<ExecutarCompraResponse?>> ExecutarCompraAsync(DateTime? data, CancellationToken cancellationToken);
}