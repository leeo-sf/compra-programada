using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface IClienteService
{
    Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken);
    Task<Result<int>> QuantidadeClientesAtivosAsync(CancellationToken cancellationToken);
    Task<Result<Cliente>> RealizarAdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken);
    Task<Result<Cliente>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<Cliente>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken);
    Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<RentabilidadeResponse>> ConsultarRentabilidadeAsync(int clienteId, CancellationToken cancellationToken);
}