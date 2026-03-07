using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaMasterService
{
    Task<Result<List<CustodiaMasterDto>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken);
    Task<Result> RegistrarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken);
    Task<Result> AtualizarResiduosAsync(List<ResiduoCustodiaMasterDto> grupoAtivos, CancellationToken cancellationToken);
    Task<Result<List<ResiduoCustodiaMasterDto>>> CapturarResiduosDeCustodiaDistribuida(List<AtivoAhCompraDto> grupoAhDistribuir, List<DistribuicaoDto> distribuicaoRealizada, CancellationToken cancellationToken);
    int SubtrairResiduosParaCompra(CustodiaMasterDto custodia, int quantidadeCompraAtivo);
}