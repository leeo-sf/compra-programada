using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class ContaMasterRepository : IContaMasterRepository
{
    private readonly AppDbContext _context;

    public ContaMasterRepository(AppDbContext context) => _context = context;

    public async Task<ContaMaster?> ObterContaMasterAsync(CancellationToken cancellationToken)
        => await _context.ContaMaster
        .Include(cm => cm.CustodiaMasters)
        .FirstOrDefaultAsync(cancellationToken);
}