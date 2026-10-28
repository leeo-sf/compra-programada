using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class CestaRecomendadaRepository : ICestaRecomendadaRepository
{
    private readonly AppDbContext _context;

    public CestaRecomendadaRepository(AppDbContext context) => _context = context;

    public void Criar(CestaRecomendada cesta)
        => _context.CestaRecomendada.Add(cesta);

    public void Atualizar(CestaRecomendada cesta)
        => _context.Entry(cesta).CurrentValues.SetValues(cesta);

    public async Task<CestaRecomendada?> ObterCestaAtualAsync(CancellationToken cancellationToken)
        => await _context.CestaRecomendada
            .Include(c => c.ComposicaoCesta)
            .FirstOrDefaultAsync(c => c.Ativa, cancellationToken);

    public async Task<List<CestaRecomendada>> ObterCestasAsync(CancellationToken cancellationToken)
        => await _context.CestaRecomendada
            .Include(c => c.ComposicaoCesta)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}