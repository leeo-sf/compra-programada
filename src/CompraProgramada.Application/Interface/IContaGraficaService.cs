using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface IContaGraficaService
{
    Task<Result> AlterarCustodiasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
    Task<Result<ContaGrafica>> GerarContaGraficaAsync(Cliente cliente, CancellationToken cancellationToken);
}