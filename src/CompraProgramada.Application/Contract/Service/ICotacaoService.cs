using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface ICotacaoService
{
    Task<Result<Cotacao>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CestaRecomendada cestaVigente, CancellationToken cancellationToken);
}