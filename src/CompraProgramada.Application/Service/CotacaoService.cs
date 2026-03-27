using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.Extensions.Logging;
using OperationResult;

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

    public async Task<Result<CotacaoDto>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CestaRecomendada cestaVigente, CancellationToken cancellationToken)
    {
        var cotacoesB3 = _cotahistParser.ParseArquivo();

        var cestaVigenteTickers = new HashSet<string>(cestaVigente.ComposicaoCesta.Select(x => x.Ticker), StringComparer.OrdinalIgnoreCase);

        var cotacoesCesta = cotacoesB3.Where(cotacao => cestaVigenteTickers.Contains(cotacao.Ticker));

        if (!cotacoesCesta.Any())
            throw new ApplicationException("Não foi possível obter a cesta recomendada nas cotações da B3.");

        var result = new CotacaoDto { DataPregao = cotacoesCesta.FirstOrDefault()!.DataPregao, Itens = cotacoesCesta.Select(x => new ComposicaoCotacaoDto(x.Ticker, x.PrecoFechamento)).ToList() };

        _logger.LogInformation("Cotações de fachamento B3 da cesta Top Five com base na data pregão {DataPregao}. Cotações: {CotacoesFechamento}", result.DataPregao, result.Itens);

        await SalvarCotacaoAsync(
            result,
            cancellationToken);

        return result;
    }

    public async Task<Result<CotacaoDto>> SalvarCotacaoAsync(CotacaoDto cotacao, CancellationToken cancellationToken)
    {
        var itensComposicao = cotacao.Itens.Select(i => ComposicaoCotacao.CriarItem(i.Ticker, i.PrecoFechamento)).ToList();
        var cotacaoSalva = await _cotacaoRepository.SalvarCotacaoAsync(Cotacao.CriarRegistro(cotacao.DataPregao, itensComposicao), cancellationToken);

        var result = new CotacaoDto
        {
            DataPregao = cotacaoSalva.DataPregao,
            Itens = cotacaoSalva.ComposicaoCotacao.Select(i => new ComposicaoCotacaoDto(i.Ticker, i.PrecoFechamento)).ToList()
        };

        return result;
    }
}