using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Domain.Contract.Service;

public interface IImpostoRendaService
{
    Task<Result<int>> PublicarIR(List<Distribuicao> distribuicoes, CancellationToken cancellationToken);
}