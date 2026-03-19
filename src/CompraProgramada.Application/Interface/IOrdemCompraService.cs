using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IOrdemCompraService
{
    Task<Result<List<OrdemCompraDto>>> ObterOrdemCompraAsync(DateTime dataEmissao, CancellationToken cancellationToken);
    Task<Result<List<OrdemCompraDto>>> EmitirOrdensDeCompraAsync(decimal valorTotalConsolidado, CancellationToken cancellationToken);
}