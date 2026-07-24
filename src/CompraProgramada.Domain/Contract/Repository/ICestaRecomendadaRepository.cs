using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface ICestaRecomendadaRepository
{
    Task<List<CestaRecomendada>> ObterCestasAsync(CancellationToken cancellationToken);
    Task<CestaRecomendada?> ObterCestaAtualAsync(CancellationToken cancellationToken);
    Task<CestaRecomendada> CriarAsync(CestaRecomendada cesta, CancellationToken cancellationToken);
    Task<CestaRecomendada> AtualizarAsync(CestaRecomendada cesta, CancellationToken cancellationToken);
}