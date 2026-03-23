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
            .Include(x => x.ContaGrafica)
            .ThenInclude(x => x.CustodiaFilhotes)
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

    public async Task<Cliente?> ObterClienteAsync(int id, CancellationToken cancellationToken)
        => await _context.Cliente
            .Include(x => x.ContaGrafica)
                .ThenInclude(conta => conta.CustodiaFilhotes)
            .Include(x => x.ContaGrafica)
                .ThenInclude(conta => conta.HistoricoCompra)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Cliente> AtualizarClienteAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        _context.Entry(cliente).CurrentValues.SetValues(cliente);
        await _context.SaveChangesAsync(cancellationToken);
        return cliente;
    }
}