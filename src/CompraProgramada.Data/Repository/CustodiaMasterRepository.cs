using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace CompraProgramada.Data.Repository;

public class CustodiaMasterRepository : ICustodiaMasterRepository
{
    private readonly AppDbContext _context;

    public CustodiaMasterRepository(AppDbContext context) => _context = context;

    public async Task<List<CustodiaMaster>> CriarAsync(List<CustodiaMaster> custodias, CancellationToken cancellationToken)
    {
        _context.CustodiaMaster.AddRange(custodias);
        await _context.SaveChangesAsync(cancellationToken);
        return custodias;
    }

    public async Task<List<CustodiaMaster>?> ObterResiduosAsync(CancellationToken cancellationToken)
        => await _context.CustodiaMaster
            .Include(x => x.ContaMaster)
            .ToListAsync();

    public async Task AtualizarResiduosAysnc(List<CustodiaMaster> custodias, CancellationToken cancellationToken)
    {
        _context.CustodiaMaster.UpdateRange(custodias);
        await _context.SaveChangesAsync(cancellationToken);
    }
}