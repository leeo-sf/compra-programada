using CompraProgramada.Shared.Dto;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.Extensions.Logging;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Service;

public class CotacaoService : ICotacaoService
{
    private readonly ILogger<CotacaoService> _logger;
    private readonly ICotacaoRepository _cotacaoRepository;
    private readonly ICotahistParserService _cotahistParser;

    public CotacaoService(ILogger<CotacaoService> logger,
        ICotacaoRepository cotacaoRepository,
        ICotahistParserService cotahistParser)
    {
        _logger = logger;
        _cotacaoRepository = cotacaoRepository;
        _cotahistParser = cotahistParser;
    }

    public async Task<Result<Cotacao>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CestaRecomendada cestaVigente, CancellationToken cancellationToken)
    {
        var cotacoesCesta = RealizarMatchFechamentoECestaRecomendada(cestaVigente);

        if (!cotacoesCesta.Any())
            return new ApplicationException("Não foi possível obter a cesta recomendada nas cotações da B3.");

        var cotacao = Cotacao.CriarRegistro(cotacoesCesta.FirstOrDefault()!.DataPregao, cotacoesCesta.Select(x => ComposicaoCotacao.CriarItem(x.Ticker, x.PrecoFechamento)).ToList());

        _logger.LogInformation("Cotações de fachamento B3 da cesta Top Five com base na data pregão {DataPregao}. Cotações: {CotacoesFechamento}", cotacao.DataPregao, cotacao.ComposicaoCotacao);

        var cotacaoSalva = await _cotacaoRepository.SalvarCotacaoAsync(cotacao, cancellationToken);

        return cotacaoSalva;
    }

    public IEnumerable<CotacaoB3Dto?> RealizarMatchFechamentoECestaRecomendada(CestaRecomendada cestaVigente)
    {
        var cotacoesB3 = _cotahistParser.ParseArquivo();

        var cestaVigenteTickers = new HashSet<string>(cestaVigente.ComposicaoCesta.Select(x => x.Ticker), StringComparer.OrdinalIgnoreCase);

        var cotacoesCesta = cotacoesB3.Where(cotacao => cestaVigenteTickers.Contains(cotacao.Ticker));

        return cotacoesCesta;
    }
}