using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class ContaMasterRepository : IContaMasterRepository
{
    private readonly AppDbContext _context;

    public ContaMasterRepository(AppDbContext context) => _context = context;

    public async Task<ContaMaster> CriarAsync(ContaMaster conta, CancellationToken cancellationToken)
    {
        _context.ContaMaster.Add(conta);
        await _context.SaveChangesAsync(cancellationToken);
        return conta;
    }

    public async Task<ContaMaster?> ObterContaMasterAsync(CancellationToken cancellationToken)
        => await _context.ContaMaster
        .Include(cm => cm.CustodiaMasters)
        .FirstOrDefaultAsync();

    public async Task<ContaMaster> AtualizarResiduosAysnc(ContaMaster conta, CancellationToken cancellationToken)
    {
        _context.ContaMaster.Update(conta);
        await _context.SaveChangesAsync(cancellationToken);
        return conta;
    }

    public async Task<ContaMaster?> ObterContaMasterAtivaAsync(CancellationToken cancellationToken)
        => await _context.ContaMaster
            .AsNoTracking()
            .Include(cm => cm.CustodiaMasters)
            .FirstOrDefaultAsync(cancellationToken);
}