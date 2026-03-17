using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICestaRecomendadaService
{
    Task<Result<CestaRecomandadaDto?>> ObterCestaAtivaAsync(CancellationToken cancellationToken);
    Task<Result<List<CestaRecomandadaDto>>> HistoricoCestasAsync(CancellationToken cancellationToken);
    Task<Result<CriarCestaRecomendadaDto>> CriarCestaAsync(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken);
    Result<(List<string> ativosRemovidos, List<string> ativosAdicionados)> ObterMudancasDeAtivos(List<ComposicaoCestaRecomendadaDto> composicaoAnterior, List<ComposicaoCestaRecomendadaDto> composicaoAtual);
    Task<Result<List<ValorAtivoConsolidadoDto>>> ValorPorAtivoConsolidado(decimal totalConsolidado, CancellationToken cancellationToken);
}