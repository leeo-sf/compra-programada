using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Data.Repository;

public class DistribuicaoRepository : IDistribuicaoRepository
{
    private readonly AppDbContext _context;

    public DistribuicaoRepository(AppDbContext context) => _context = context;

    public async Task SalvarDistribuicoesAsync(List<Distribuicao> distribuicoes, CancellationToken cancellationToken)
    {
        _context.Distribuicao.AddRange(distribuicoes);
        await _context.SaveChangesAsync();
    }
}