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
        _context.CestaRecomendada.Add(cesta);
        await _context.SaveChangesAsync(cancellationToken);
        return cesta;
    }

    public async Task<CestaRecomendada> AtualizarAsync(CestaRecomendada cesta, CancellationToken cancellationToken)
    {
        _context.Entry(cesta).CurrentValues.SetValues(cesta);
        await _context.SaveChangesAsync(cancellationToken);
        return cesta;
    }

    public async Task<CestaRecomendada?> ObterCestaAtivaAsync(CancellationToken cancellationToken)
        => await _context.CestaRecomendada
            .Include(c => c.ComposicaoCesta)
            .FirstOrDefaultAsync(c => c.Ativa, cancellationToken);

    public async Task<List<CestaRecomendada>> ObterTodasCestasAsync(CancellationToken cancellationToken)
        => await _context.CestaRecomendada
            .Include(c => c.ComposicaoCesta)
            .AsNoTracking()
            .ToListAsync();
}