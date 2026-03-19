using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IDistribuicaoService
{
    Task<(List<DistribuicaoDto>, List<AtivoAhCompraDto>)> RealizarDistribuicoesAsync(List<ClienteDto> clientesAtivos, DateTime dataExecucao, CancellationToken cancellationToken);
}