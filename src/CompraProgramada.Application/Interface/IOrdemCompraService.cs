using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IOrdemCompraService
{
    Task<Result<List<OrdemCompraDto>>> EmitirOrdensDeCompraAsync(List<OrdemCompraDto> ordensCompra, CancellationToken cancellationToken);
}