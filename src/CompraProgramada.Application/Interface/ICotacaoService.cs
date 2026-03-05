using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICotacaoService
{
    Task<Result<CotacaoDto>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CancellationToken cancellationToken);
    Task<Result<CotacaoDto>> ObterCotacaoAsync(DateTime dataPregao, CancellationToken cancellationToken);
    Task<Result<CotacaoDto>> SalvarCotacaoAsync(CotacaoDto cotacao, CancellationToken cancellationToken);
}