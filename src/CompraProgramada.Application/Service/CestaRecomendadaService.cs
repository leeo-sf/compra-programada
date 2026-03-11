using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CestaRecomendadaService : ICestaRecomendadaService
{
    private readonly ICestaRecomendadaRepository _cestaRepository;
    private const string QUANTIDADE_INVALIDA_CODE = "QUANTIDADE_ATIVOS_INVALIDA";
    private const string PERCENTUAIS_INVALIDO_CODE = "PERCENTUAIS_INVALIDOS";

    public CestaRecomendadaService(ICestaRecomendadaRepository cestaRepository) => _cestaRepository = cestaRepository;

    public async Task<Result<CriarAlterarCestaDto>> CriarCestaAsync(CriarAlterarCestaRequest request, CancellationToken cancellationToken)
    {
        var cestaAnterior = await DesativaCestaAtualAsync(cancellationToken);

        var itensCesta = request.Itens.Select(i => ComposicaoCesta.CriaItemNaCesta(i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRepository.CriarAsync(CestaRecomendada.CriarCesta(request.Nome, itensCesta), cancellationToken);

        return new CriarAlterarCestaDto(
            cestaAnterior is not null ? true : false,
            GerarCestaDto(cestaCriada),
            cestaAnterior is not null ? cestaAnterior : null);
    }

    public async Task<Result<CestaRecomandadaDto?>> ObterCestaAtivaAsync(CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cesta is null)
            return new ApplicationException("Nenhuma Cesta Top Five ativa no momento.");

        return GerarCestaDto(cesta);
    }

    public async Task<Result<IEnumerable<CestaRecomandadaDto>>> HistoricoCestasAsync(CancellationToken cancellationToken)
    {
        var cestas = await _cestaRepository.ObterTodasCestasAsync(cancellationToken);

        return cestas.Select(c => GerarCestaDto(c))
            .OrderByDescending(c => c.Id)
            .ToList();
    }

    public Result<(List<string> ativosRemovidos, List<string> ativosAdicionados)> ObterMudancasDeAtivos(List<ComposicaoCestaRecomendadaDto> composicaoAnterior, List<ComposicaoCestaRecomendadaDto> composicaoAtual)
    {
        var tickersAnteriores = composicaoAnterior.Select(c => c.Ticker);
        var tickersAtual = composicaoAtual.Select(c => c.Ticker);

        var ativosRemovidos = tickersAnteriores
            .Except(tickersAtual)
            .ToList();

        var ativosAdicionados = tickersAtual
            .Except(tickersAnteriores)
            .ToList();

        return (ativosRemovidos, ativosAdicionados);
    }

    private async Task<CestaRecomandadaDto?> DesativaCestaAtualAsync(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cestaAtual is not null)
        {
            cestaAtual.DesativarCesta();

            await _cestaRepository.AtualizarAsync(cestaAtual, cancellationToken);

            return GerarCestaDto(cestaAtual);
        }

        return null;
    }

    public Result<List<ValorAtivoConsolidadoDto>> ValorPorAtivoConsolidado(CestaRecomandadaDto cesta, decimal totalConsolidado)
    {
        var valorPorAtivoConsolidado = cesta.Itens
            .Select(ativo => new ValorAtivoConsolidadoDto { Ticker = ativo.Ticker, ValorDeCompraPorAtivo = totalConsolidado * (ativo.Percentual / 100) })
            .ToList();

        return valorPorAtivoConsolidado;
    }

    private CestaRecomandadaDto GerarCestaDto(CestaRecomendada cesta)
        => new CestaRecomandadaDto(
            cesta.Id,
            cesta.Nome,
            cesta.DataCriacao,
            cesta.DataDesativacao,
            cesta.Ativa,
            cesta.ComposicaoCesta.Select(cc => new ComposicaoCestaRecomendadaDto(
                cc.Id,
                cc.Id,
                cc.Ticker,
                cc.Percentual
            )).ToList()
        );
}