using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IContaGraficaRepository
{
    Task<ContaGrafica> CriarAsync(ContaGrafica conta, CancellationToken cancellationToken);
    Task RegistrarHistoricoCompraAysnc(List<HistoricoCompra> compras, CancellationToken cancellationToken);
    Task<List<ContaGrafica>> ObterContasAtivas(CancellationToken cancellationToken);
    Task<List<CustodiaFilhote>> AtualizarCustodiasAsync(List<CustodiaFilhote> custodias, CancellationToken cancellationToken);
}