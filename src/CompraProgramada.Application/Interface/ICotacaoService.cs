using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICotacaoService
{
    Task<Result<Cotacao>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CestaRecomendada cestaVigente, CancellationToken cancellationToken);
}