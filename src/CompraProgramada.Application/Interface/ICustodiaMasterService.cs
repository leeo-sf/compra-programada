using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaMasterService
{
    Task<Result<List<CustodiaMasterDto>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken);
    Task<Result> RegistrarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken);
    Task<Result> AtualizarResiduosAsync(List<GrupoAtivoCompraDto> grupoAtivos, CancellationToken cancellationToken);
    Task<Result> CapturarResiduosDeCustodiaDistribuida(List<GrupoAtivoCompraDto> grupoAhDistribuir, List<DistribuicaoDto> distribuicaoRealizada, CancellationToken cancellationToken);
    int SubtrairResiduosParaCompra(CustodiaMasterDto custodia, int quantidadeCompraAtivo);
}