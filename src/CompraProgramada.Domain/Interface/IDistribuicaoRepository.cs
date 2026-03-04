using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IDistribuicaoRepository
{
    Task SalvarDistribuicoesAsync(List<Distribuicao> distribuicoes, CancellationToken cancellationToken);
}