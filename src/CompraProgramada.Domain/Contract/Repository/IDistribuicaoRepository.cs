using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IDistribuicaoRepository
{
    Task SalvarDistribuicoesAsync(List<Distribuicao> distribuicoes, CancellationToken cancellationToken);
}