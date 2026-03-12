using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICotacaoService
{
    Task<Result<List<FechamentoAtivoB3Dto>>> ObterCombinacoesFechamentoECompraAtivoAsync(decimal totalConsolidado, CancellationToken cancellationToken);
    Task<Result<CotacaoDto>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CancellationToken cancellationToken);
}