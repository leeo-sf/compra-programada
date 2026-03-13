using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IClienteService
{
    Task<Result<List<ClienteDto>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken);
    Task<Result<int>> QuantidadeClientesAtivosAsync(CancellationToken cancellationToken);
    Task<Result<ClienteDto>> RealizarAdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken);
    Task<Result<ClienteDto>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<ClienteDto>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken);
    Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<RentabilidadeResponse>> ConsultarRentabilidadeAsync(int clienteId, CancellationToken cancellationToken);
}