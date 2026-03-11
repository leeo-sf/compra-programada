using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IContaGraficaService
{
    Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result> RegistrarComprasAsync(List<HistoricoCompraDto> compras, CancellationToken cancellationToken);
    Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiasContasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken);
}