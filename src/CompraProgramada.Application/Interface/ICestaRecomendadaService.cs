using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICestaRecomendadaService
{
    Task<Result<CestaRecomendada?>> ObterCestaAtivaAsync(CancellationToken cancellationToken);
    Task<Result<List<CestaRecomendada>>> HistoricoCestasAsync(CancellationToken cancellationToken);
    Task<Result<CriarCestaRecomendadaDto>> CriarCestaAsync(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken);
    Task<Result<List<ValorAtivoConsolidadoDto>>> ValorPorAtivoConsolidado(decimal totalConsolidado, CancellationToken cancellationToken);
    (List<string> ativosRemovidos, List<string> ativosAdicionados) ObterMudancasDeAtivos(List<ComposicaoCestaRecomendadaDto> composicaoAnterior, List<ComposicaoCestaRecomendadaDto> composicaoAtual);
}