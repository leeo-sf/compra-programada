using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface IImpostoRendaService
{
    Task<Result<int>> PublicarIR(List<Distribuicao> distribuicoes, CancellationToken cancellationToken);
}