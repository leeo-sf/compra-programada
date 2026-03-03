using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface ICotacaoRepository
{
    Task<Cotacao?> ObterCotacaoAsync(DateTime dataPregao, CancellationToken cancellationToken);
    Task<Cotacao> SalvarCotacaoAsync(Cotacao cotacao,CancellationToken cancellationToken);
}