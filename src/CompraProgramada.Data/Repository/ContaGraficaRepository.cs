using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<ContaGrafica>> ObterContasAtivas(CancellationToken cancellationToken)
    {
        var clientes = await _context.Cliente
            .Where(c => c.Ativo)
            .Include(c => c.ContaGrafica)
                .ThenInclude(c => c.CustodiaFilhotes)
            .ToListAsync(cancellationToken);

        return clientes.Select(x => x.ContaGrafica).ToList();
    }

    public async Task<List<CustodiaFilhote>> AtualizarCustodiasAsync(List<CustodiaFilhote> custodias, CancellationToken cancellationToken)
    {
        _context.Entry(custodias).CurrentValues.SetValues(custodias);
        await _context.SaveChangesAsync(cancellationToken);
        return custodias;
    }
}