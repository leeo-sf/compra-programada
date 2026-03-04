using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface IClienteService
{
    Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken);
    Task<Result<int>> QuantidadeAtivosAsync(CancellationToken cancellationToken);
    Task<Result<Cliente>> AdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken);
    Task<Result> RealizaDistribuicaoAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
}