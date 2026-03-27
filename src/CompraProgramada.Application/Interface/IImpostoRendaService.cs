using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IImpostoRendaService
{
    Task<Result<int>> PublicarIR(List<Distribuicao> distribuicoes, CancellationToken cancellationToken);
}