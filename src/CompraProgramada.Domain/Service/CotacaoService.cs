using CompraProgramada.Shared.Dto;
using CompraProgramada.Domain.Entity;
using Microsoft.Extensions.Logging;
using OperationResult;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Handler.Api;

namespace CompraProgramada.Domain.Service;

public class CotacaoService : ICotacaoService
{
    private readonly ILogger<CotacaoService> _logger;
    private readonly ICotacaoRepository _cotacaoRepository;
    private readonly ICotahistParserService _cotahistParser;

    public CotacaoService(
        ILogger<CotacaoService> logger,
        ICotacaoRepository cotacaoRepository,
        ICotahistParserService cotahistParser)
    {
        _logger = logger;
        _cotacaoRepository = cotacaoRepository;
        _cotahistParser = cotahistParser;
    }

    public async Task<Result<Cotacao>> ObterCotacoesDaCestaRecomendadaAsync(CestaRecomendada cestaVigente, CancellationToken cancellationToken)
    {
        var cotacao = await _cotacaoRepository.ObterCotacaoAsync(DateOnly.FromDateTime(DateTime.Now), cancellationToken);
        if (cotacao is not null)
        {
            var teveMudanca = AdministradorHandler.ObterMudancasDeAtivos(
                [.. cotacao.ComposicaoCotacao.Select(c => c.Ticker)],
                [.. cestaVigente.ComposicaoCesta.Select(c => c.Ticker)])
                is { ativosAdicionados.Count: > 0 };

            if (!teveMudanca)
                return cotacao;
        }

        var cotacoesCesta = RealizarMatchB3ECestaRecomendada(cestaVigente);

        if (cotacoesCesta is null || !cotacoesCesta.Any())
            return new ApplicationException("Não foi possível obter a cotação da cesta recomendada na B3.");

        Cotacao cotacaoAhRegistrar = Cotacao.CriarRegistro(
            DateOnly.FromDateTime(cotacoesCesta.Select(x => x.DataPregao).First()),
            [.. cotacoesCesta.Select(x => ComposicaoCotacao.CriarItem(x.Ticker, x.PrecoFechamento))]);

        _logger.LogInformation("Cotações de fachamento B3 da cesta Top Five com base na data pregão {DataPregao}. Cotações: {CotacoesFechamento}", cotacaoAhRegistrar.DataPregao, cotacaoAhRegistrar.ComposicaoCotacao);

        var cotacaoSalva = await _cotacaoRepository.SalvarCotacaoAsync(cotacaoAhRegistrar, cancellationToken);

        return cotacaoSalva;
    }

    internal IEnumerable<CotacaoB3Dto>? RealizarMatchB3ECestaRecomendada(CestaRecomendada cestaVigente)
    {
        var cotacoesB3 = _cotahistParser.ParseArquivo();
        if (cotacoesB3 is null)
            return default;

        var cestaHashTickers = cestaVigente.ComposicaoCesta.Select(x => x.Ticker)
            .Where(ticker => !string.IsNullOrWhiteSpace(ticker))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return cotacoesB3.Where(cotacao => cestaHashTickers.Contains(cotacao.Ticker));
    }
}