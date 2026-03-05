using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IDistribuicaoService
{
    Task<Result<List<DistribuicaoDto>>> RealizarDistribuicaoAtivoPorCliente(List<ClienteDto> clientesAtivos, decimal totalConsolidado, CancellationToken cancellationToken);
}