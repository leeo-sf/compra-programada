using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Data.Repository;

public class ContaGraficaRepository : IContaGraficaRepository
{
    private readonly AppDbContext _context;

    public ContaGraficaRepository(AppDbContext context) => _context = context;

    public async Task<ContaGrafica> CriarAsync(ContaGrafica conta, CancellationToken cancellationToken)
    {
        _context.ContaGrafica.Add(conta);
        await _context.SaveChangesAsync();
        return conta;
    }

    public async Task AtualizarCustodiasAysnc(List<ContaGrafica> contas, CancellationToken cancellationToken)
    {
        _context.ContaGrafica.UpdateRange(contas);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RegistrarHistoricoCompraAysnc(List<HistoricoCompra> compras, CancellationToken cancellationToken)
    {
        _context.HistoricoCompra.AddRange(compras);
        await _context.SaveChangesAsync();
    }
}