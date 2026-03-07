using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICompraService
{
    Task ExecutarCompraAsync(DateTime? data, CancellationToken cancellationToken);
    Task<Result<List<OrdemCompraDto>>> SeparacaoLoteDeCompra(List<FechamentoAtivoB3Dto> fechamentoAtivos, DateTime dataExecucao, CancellationToken cancellationToken);
}