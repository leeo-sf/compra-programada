using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface ICestaRecomendadaService
{
    Task<Result<CestaRecomendada?>> ObterCestaAtivaAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<CestaRecomendada>>> HistoricoCestasAsync(CancellationToken cancellationToken);
    Task<Result<CriarAlterarCestaDto>> CriarCestaAsync(CriarAlterarCestaRequest request, CancellationToken cancellationToken);
    Result<(List<string> ativosRemovidos, List<string> ativosAdicionados)> ObterMudancasDeAtivos(List<ComposicaoCesta> composicaoAnterior, List<ComposicaoCesta> composicaoAtual);
}