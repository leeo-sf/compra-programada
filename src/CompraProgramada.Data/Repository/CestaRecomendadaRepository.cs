using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class CestaRecomendadaRepository : ICestaRecomendadaRepository
{
    private readonly AppDbContext _context;

    public CestaRecomendadaRepository(AppDbContext context) => _context = context;

    public async Task<CestaRecomendada> CriarAsync(CestaRecomendada cesta, CancellationToken cancellationToken)
    {
        _context.Cesta.Add(cesta);
        await _context.SaveChangesAsync(cancellationToken);
        return cesta;
    }

    public async Task<CestaRecomendada> AtualizarAsync(CestaRecomendada cestaAnterior, CestaRecomendada novaCesta, CancellationToken cancellationToken)
    {
        _context.Entry(cestaAnterior).CurrentValues.SetValues(novaCesta);
        await _context.SaveChangesAsync(cancellationToken);
        return novaCesta;
    }

    public async Task<CestaRecomendada?> ObterCestaAtivaAsync(CancellationToken cancellationToken)
        => await _context.Cesta
            .Include(c => c.ComposicaoCesta)
            .FirstOrDefaultAsync(c => c.Ativa, cancellationToken);

    public async Task<List<CestaRecomendada>> ObterTodasCestasAsync(CancellationToken cancellationToken)
        => await _context.Cesta
            .Include(c => c.ComposicaoCesta)
            .AsNoTracking()
            .ToListAsync();
}