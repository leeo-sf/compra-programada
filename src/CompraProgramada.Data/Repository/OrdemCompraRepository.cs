using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Data.Repository;

public class OrdemCompraRepository : IOrdemCompraRepository
{
    private readonly AppDbContext _context;

    public OrdemCompraRepository(AppDbContext context) => _context = context;

    public async Task<List<OrdemCompra>> SalvarOrdensDeCompra(List<OrdemCompra> ordemCompra, CancellationToken cancellationToken)
    {
        _context.OrdemCompra.AddRange(ordemCompra);
        await _context.SaveChangesAsync();
        return ordemCompra;
    }
}