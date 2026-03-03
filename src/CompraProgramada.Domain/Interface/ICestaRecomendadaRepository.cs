using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface ICestaRecomendadaRepository
{
    Task<List<CestaRecomendada>> ObterTodasCestasAsync(CancellationToken cancellationToken);
    Task<CestaRecomendada> CriarAsync(CestaRecomendada cesta, CancellationToken cancellationToken);
    Task<CestaRecomendada> AtualizarAsync(CestaRecomendada cestaAnterior, CestaRecomendada novaCesta, CancellationToken cancellationToken);
    Task<CestaRecomendada?> ObterCestaAtivaAsync(CancellationToken cancellationToken);
}