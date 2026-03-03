using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class ContaRepository : IContaRepository
{
    private readonly AppDbContext _context;

    public ContaRepository(AppDbContext context) => _context = context;

    public async Task<T> CreateAsync<T>(T conta, CancellationToken cancellationToken)
    {
        _context.Add(conta!);
        await _context.SaveChangesAsync(cancellationToken);
        return conta;
    }
}