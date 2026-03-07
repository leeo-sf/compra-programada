using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IImpostoRendaService
{
    Task<Result<int>> CalcularIRDedoDuro(List<DistribuicaoDto> distribuicoes, CancellationToken cancellationToken);
}