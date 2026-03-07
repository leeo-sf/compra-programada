using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IClienteService
{
    Task<Result<List<ClienteDto>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken);
    Task<Result<int>> QuantidadeAtivosAsync(CancellationToken cancellationToken);
    Task<Result<ClienteDto>> AdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken);
    Result<decimal> TotalConsolidade(List<ClienteDto> clientesAtivos);
    Task<Result<ClienteDto>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<ClienteDto>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken);
    Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<RentabilidadeResponse>> ConsultarRentabilidadeAsync(int clienteId, CancellationToken cancellationToken);
}