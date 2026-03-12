using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IDistribuicaoService
{
    Task<Result<List<AtivoAhCompraDto>>> DistribuirGrupoAtivoAsync(List<ClienteDto> clientesAtivos, decimal totalConsolidado, DateTime dataExeucao, CancellationToken cancellationToken);
    Task<Result<List<DistribuicaoDto>>> DistribuirCustodiasAsync(List<ClienteDto> clientes, List<AtivoAhCompraDto> grupoAtivoCompra, decimal totalConsolidado, DateTime dataExeucao, CancellationToken cancellationToken);
    Task<Result> SalvarDistribuicoesAsync(List<DistribuicaoDto> ditribuicoes, List<OrdemCompraDto> ordensCompraAtivos, CancellationToken cancellationToken);
}