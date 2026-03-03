using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface IContaService
{
    Task<Result<ContaGrafica>> GerarContaGraficaAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<Result<ContaMaster>> GerarContaMasterAsync(Cliente cliente, CancellationToken cancellationToken);
}