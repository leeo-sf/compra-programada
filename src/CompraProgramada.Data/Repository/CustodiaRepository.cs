using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Data.Repository;

public class CustodiaRepository : ICustodiaRepository
{
    private readonly AppDbContext _context;

    public CustodiaRepository(AppDbContext context) => _context = context;

    public async Task<T> CreateAsync<T>(T custodia, CancellationToken cancellationToken)
    {
        _context.Add(custodia!);
        await _context.SaveChangesAsync(cancellationToken);
        return custodia;
    }
}