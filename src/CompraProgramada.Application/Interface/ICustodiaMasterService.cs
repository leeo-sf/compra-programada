using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaMasterService
{
    Task<Result> RegistrarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken);
}