using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Repository;

public class CotacaoRepository : ICotacaoRepository
{
    private readonly AppDbContext _context;

    public CotacaoRepository(AppDbContext context) => _context = context;

    public async Task<Cotacao?> ObterCotacaoAsync(DateOnly dataRegistro, CancellationToken cancellationToken)
        => await _context.Cotacao
            .Include(x => x.ComposicaoCotacao)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => DateOnly.FromDateTime(x.DataCriacao) == dataRegistro, cancellationToken);

    public async Task<Cotacao> SalvarCotacaoAsync(Cotacao cotacao, CancellationToken cancellationToken)
    {
        _context.Cotacao.Add(cotacao);
        await _context.SaveChangesAsync(cancellationToken);
        return cotacao;
    }
}