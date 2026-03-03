using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface ICotacaoService
{
    Task<Result<CotacaoDto>> ObterCotacaoAsync(DateTime dataPregao, CancellationToken cancellationToken);
    Task<Result<CotacaoDto>> SalvarCotacaoAsync(CotacaoDto cotacao, CancellationToken cancellationToken);
}