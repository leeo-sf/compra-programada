using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CestaRecomendadaService : ICestaRecomendadaService
{
    private readonly ILogger<CestaRecomendadaService> _logger;
    private readonly ICestaRecomendadaRepository _cestaRepository;

    public CestaRecomendadaService(ILogger<CestaRecomendadaService> logger,
        ICestaRecomendadaRepository cestaRepository)
    {
        _logger = logger;
        _cestaRepository = cestaRepository;
    }

    public async Task<Result<CriarCestaRecomendadaDto>> CriarCestaAsync(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de registro de uma nova cesta...");

        var cestaAnterior = await DesativaCestaAtualAsync(cancellationToken);

        var itensCesta = request.Itens.Select(i => ComposicaoCesta.CriaItemNaCesta(i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRepository.CriarAsync(CestaRecomendada.CriarCesta(request.Nome, itensCesta), cancellationToken);

        _logger.LogInformation("Cesta registrada na base de dados {Cesta}", cestaCriada);

        return new CriarCestaRecomendadaDto(
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

        _logger.LogInformation("Mudanças de ativos identificados {Removidos} - {Adicionados}", ativosRemovidos, ativosAdicionados);

        return (ativosRemovidos, ativosAdicionados);
    }

    public async Task<Result<List<ValorAtivoConsolidadoDto>>> ValorPorAtivoConsolidado(decimal totalConsolidado, CancellationToken cancellationToken)
    {
        var cestaVigente = await ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var valorPorAtivoConsolidado = cestaVigente.Value.Itens
            .Select(ativo => new ValorAtivoConsolidadoDto(ativo.Ticker, totalConsolidado * (ativo.Percentual / 100)))
            .ToList();

        return valorPorAtivoConsolidado;
    }

    private async Task<CestaRecomandadaDto?> DesativaCestaAtualAsync(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cestaAtual is not null)
        {
            cestaAtual.DesativarCesta();

            await _cestaRepository.AtualizarAsync(cestaAtual, cancellationToken);

            _logger.LogInformation("Cesta atual desativada {Cesta}", cestaAtual);

            return GerarCestaDto(cestaAtual);
        }

        _logger.LogInformation("Nenhuma cesta foi encontrada na base de dados para desatiação");

        return null;
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