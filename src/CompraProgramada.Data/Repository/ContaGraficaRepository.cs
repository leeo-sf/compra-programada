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

    public async Task<List<ContaGrafica>> AtualizarContasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken)
    {
        _context.ContaGrafica.UpdateRange(contas);
        await _context.SaveChangesAsync(cancellationToken);
        return contas;
    }
}