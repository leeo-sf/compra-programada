using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface ICestaRecomendadaRepository
{
    Task<List<CestaRecomendada>> ObterCestasAsync(CancellationToken cancellationToken);
    Task<CestaRecomendada?> ObterCestaAtualAsync(CancellationToken cancellationToken);
    void Criar(CestaRecomendada cesta);
    void Atualizar(CestaRecomendada cesta);
}