using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IContaGraficaRepository
{
    Task<ContaGrafica> CriarAsync(ContaGrafica conta, CancellationToken cancellationToken);
    Task<List<ContaGrafica>> AtualizarContasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
}