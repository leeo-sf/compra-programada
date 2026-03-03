using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context) => _context = context;

    public async Task<List<Cliente>> ObterClientesAtivosAsync(CancellationToken cancellationToken)
        => await _context.Cliente
            .AsNoTracking()
            .Where(c => c.Ativo)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExisteAsync(string cpf, CancellationToken cancellationToken)
        => await _context.Cliente.AnyAsync(x => x.Cpf == cpf, cancellationToken);

    public async Task<Cliente> CriarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync(cancellationToken);
        return cliente;
    }

    public async Task<int> QuantidadeAtivosAsync(CancellationToken cancellationToken)
        => await _context.Cliente
            .AsNoTracking()
            .CountAsync(c => c.Ativo);
}