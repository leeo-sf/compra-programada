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
    private readonly IFileService _fileService;
    private readonly ICotahistParserService _cotahistParser;
    private readonly ICestaRecomendadaService _cestaService;

    public CotacaoService(ILogger<CotacaoService> logger,
        ICotacaoRepository cotacaoRepository,
        IFileService fileService,
        ICotahistParserService cotahistParser,
        ICestaRecomendadaService cestaService)
    {
        _logger = logger;
        _cotacaoRepository = cotacaoRepository;
        _fileService = fileService;
        _cotahistParser = cotahistParser;
        _cestaService = cestaService;
    }

    public async Task<Result<CotacaoDto>> ObterCotacaoAsync(DateTime dataPregao, CancellationToken cancellationToken)
    {
        var cotacao = await _cotacaoRepository.ObterCotacaoAsync(dataPregao, cancellationToken);

        if (cotacao is null)
            return new ApplicationException($"Não existe cotação para a data informada {dataPregao:dd/mm/yyyy}");

        var result = new CotacaoDto
        {
            DataPregao = cotacao.DataPregao,
            Itens = cotacao.ComposicaoCotacao.Select(i => new ComposicaoCotacaoDto(i.Ticker, i.PrecoFechamento)).ToList()
        };

        return result;
    }

    public async Task<Result<CotacaoDto>> ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CancellationToken cancellationToken)
    {
        var caminhoArquivoUltimoPregao = _fileService.ObterCaminhoCompletoArquivoCotacoes();

        var cotacoesB3 = _cotahistParser.ParseArquivo(caminhoArquivoUltimoPregao);

        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (!cestaVigente.IsSuccess)
            throw new ApplicationException(cestaVigente.Exception.Message);

        var cestaVigenteTickers = new HashSet<string>(cestaVigente.Value.Itens.Select(x => x.Ticker), StringComparer.OrdinalIgnoreCase);

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
        var itensComposicao = cotacao.Itens.Select(i => new ComposicaoCotacao(0, 0, i.Ticker, i.PrecoFechamento)).ToList();
        var cotacaoSalva = await _cotacaoRepository.SalvarCotacaoAsync(new(0, cotacao.DataPregao) { ComposicaoCotacao = itensComposicao }, cancellationToken);

        var result = new CotacaoDto
        {
            DataPregao = cotacaoSalva.DataPregao,
            Itens = cotacaoSalva.ComposicaoCotacao.Select(i => new ComposicaoCotacaoDto(i.Ticker, i.PrecoFechamento)).ToList()
        };

        return result;
    }
}