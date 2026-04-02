using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface ICestaRecomendadaService
{
    Task<Result<CestaRecomendada?>> ObterCestaAtivaAsync(CancellationToken cancellationToken);
    Task<Result<List<CestaRecomendada>>> HistoricoCestasAsync(CancellationToken cancellationToken);
    Task<Result<CriarCestaRecomendadaDto>> CriarCestaAsync(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken);
    Task<Result<List<ValorAtivoConsolidadoDto>>> ValorPorAtivoConsolidado(decimal totalConsolidado, CancellationToken cancellationToken);
    (List<string> ativosRemovidos, List<string> ativosAdicionados) ObterMudancasDeAtivos(List<ComposicaoCestaDto> composicaoAnterior, List<ComposicaoCestaDto> composicaoAtual);
}