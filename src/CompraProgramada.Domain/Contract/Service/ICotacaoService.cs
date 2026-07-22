using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Domain.Contract.Service;

public interface ICotacaoService
{
    Task<Result<Cotacao>> ObterCotacoesDaCestaRecomendadaAsync(CestaRecomendada cestaVigente, CancellationToken cancellationToken);
}