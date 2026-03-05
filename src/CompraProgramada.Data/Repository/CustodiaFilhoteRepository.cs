using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Data.Repository;

public class CustodiaFilhoteRepository : ICustodiaFilhoteRepository
{
    private readonly AppDbContext _context;

    public CustodiaFilhoteRepository(AppDbContext context) => _context = context;

    public async Task<List<CustodiaFilhote>> AtualizarCustodiasAsync(List<CustodiaFilhote> custodias, CancellationToken cancellationToken)
    {
        _context.CustodiaFilhote.UpdateRange(custodias);
        await _context.SaveChangesAsync(cancellationToken);
        return custodias;
    }
}