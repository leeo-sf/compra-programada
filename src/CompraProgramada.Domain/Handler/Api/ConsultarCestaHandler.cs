using CompraProgramada.Domain.Contract.Handler;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Domain.Handler.Api;

public class ConsultarCestaHandler
    : IRequestHandler<CestaAtualRequest, Result<CestaRecomendadaDto>>,
        IRequestHandler<CestaHistoricoRequest, Result<HistoricoCestasResponse>>, IApiRequestHandler
{
    private readonly ILogger<ConsultarCestaHandler> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly CestaRecomendadaMapper _mapper;

    public ConsultarCestaHandler(
        ILogger<ConsultarCestaHandler> logger,
        ICestaRecomendadaRepository cestaRecomendadaRepository,
        ICotacaoService cotacaoService,
        CestaRecomendadaMapper mapper)
    {
        _logger = logger;
        _cestaRecomendadaRepository = cestaRecomendadaRepository;
        _cotacaoService = cotacaoService;
        _mapper = mapper;
    }

    public async Task<Result<CestaRecomendadaDto>> Handle(CestaAtualRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando consulta da cesta atual.");

        var cesta = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);

        if (cesta is null)
            return new ApplicationException("Nenhuma Cesta Top Five ativa no momento.");

        var cotacoesResult = await _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(cesta, cancellationToken);
        if (!cotacoesResult.IsSuccess)
            return cotacoesResult.Exception;

        var cotacao = cotacoesResult.Value;

        return new CestaRecomendadaDto
        {
            CestaId = cesta.Id,
            Nome = cesta.Nome,
            DataCriacao = cesta.DataCriacao,
            Ativa = cesta.Ativa,
            Itens = [.. cesta.ComposicaoCesta.Select(ativo => new ComposicaoCestaDto
            {
                Ticker = ativo.Ticker,
                Percentual = ativo.Percentual,
                CotacaoAtual = cotacao.ComposicaoCotacao.Where(c => c.Ticker == ativo.Ticker).Select(c => c.PrecoFechamento).FirstOrDefault()
            })]
        };
    }

    public async Task<Result<HistoricoCestasResponse>> Handle(CestaHistoricoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de consulta de histórico de cestas.");

        var cestas = await _cestaRecomendadaRepository.ObterCestasAsync(cancellationToken);

        var cestasDto = cestas.Select(_mapper.ToResponse).ToList();

        return new HistoricoCestasResponse(cestasDto);
    }
}