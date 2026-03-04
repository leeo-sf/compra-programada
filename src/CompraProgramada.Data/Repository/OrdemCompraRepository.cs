using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Data.Repository;

public class OrdemCompraRepository : IOrdemCompraRepository
{
    private readonly AppDbContext _context;

    public OrdemCompraRepository(AppDbContext context) => _context = context;

    public Task SalvarOrdemDeCompra(OrdemCompra ordemCompra, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}