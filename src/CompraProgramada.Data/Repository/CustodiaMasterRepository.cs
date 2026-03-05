using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

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
            .Where(cm => cm.QuantidadeResiduo > 0)
            .ToListAsync();

    public async Task AtualizarResiduosAysnc(List<CustodiaMaster> conta, CancellationToken cancellationToken)
    {
        _context.Entry(conta).CurrentValues.SetValues(conta);
        await _context.SaveChangesAsync(cancellationToken);
    }
}