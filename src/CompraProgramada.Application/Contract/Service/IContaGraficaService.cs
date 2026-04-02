using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface IContaGraficaService
{
    Task<Result<ContaGrafica>> GerarContaGraficaAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<Result<List<ContaGrafica>>> AtualizarContasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
}