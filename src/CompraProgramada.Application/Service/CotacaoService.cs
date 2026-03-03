using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class CotacaoService : ICotacaoService
{
    private readonly ICotacaoRepository _cotacaoRepository;

    public CotacaoService(ICotacaoRepository cotacaoRepository) => _cotacaoRepository = cotacaoRepository;

    public async Task<Result<CotacaoDto>> ObterCotacaoAsync(DateTime dataPregao, CancellationToken cancellationToken)
    {
        var cotacao = await _cotacaoRepository.ObterCotacaoAsync(dataPregao, cancellationToken);

        if (cotacao is null)
            return Result<CotacaoDto>.Fail(new ApplicationException($"Não existe cotação para a data informada {dataPregao:dd/mm/yyyy}"));

        var result = new CotacaoDto
        {
            DataPregao = cotacao.DataPregao,
            Itens = cotacao.ComposicaoCotacao.Select(i => new ComposicaoCotacaoDto(i.Ticker, i.PrecoFechamento)).ToList()
        };

        return Result<CotacaoDto>.Ok(result);
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

        return Result<CotacaoDto>.Ok(result);
    }
}