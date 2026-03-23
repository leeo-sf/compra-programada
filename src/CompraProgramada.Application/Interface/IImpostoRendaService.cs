using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IImpostoRendaService
{
    Task<Result<int>> CalcularIRDedoDuro(List<Distribuicao> distribuicoes, CancellationToken cancellationToken);
}