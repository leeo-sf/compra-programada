using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaMasterService
{
    Task<Result<List<CustodiaMasterDto>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken);
    Task<Result> RegistrarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken);
    Task<Result> AjustarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken);
}