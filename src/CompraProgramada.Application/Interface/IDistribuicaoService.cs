using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IDistribuicaoService
{
    Task<Result<List<Distribuicao>>> DistribuirParaCustodiasAsync(List<Cliente> clientesAtivos, List<OrdemCompra> ordensCompra, DateTime dataExecucao, CancellationToken cancellationToken);
}