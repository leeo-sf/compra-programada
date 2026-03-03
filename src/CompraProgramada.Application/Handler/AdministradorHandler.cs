using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Handler;

public class AdministradorHandler
    : IRequestHandler<CriarAlterarCestaRequest, Result<CriarAlterarCestaResponse>>,
        IRequestHandler<CestaAtualRequest, Result<CestaResponse>>,
        IRequestHandler<CestaHistoricoRequest, Result<List<CestaResponse>>>
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaService _cestaService;

    public AdministradorHandler(ILogger<AdministradorHandler> logger,
        ICestaRecomendadaService cestaService)
    {
        _logger = logger;
        _cestaService = cestaService;
    }

    public async Task<Result<CriarAlterarCestaResponse>> Handle(CriarAlterarCestaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de criação de cesta: {Request}", request);

        var result = await _cestaService.CriarCestaAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao processar criação da cesta: {Exception}", result.Exception);
            return Result<CriarAlterarCestaResponse>.Fail(result.Exception);
        }

        return Result<CriarAlterarCestaResponse>.Ok(result.Value!);
    }

    public async Task<Result<CestaResponse>> Handle(CestaAtualRequest request, CancellationToken cancellationToken)
    {
        var result = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (result is null)
            return Result<CestaResponse>.Fail(new ApplicationException("Não existe cesta ativa no momento"));

        var cesta = result.Value!;

        return Result<CestaResponse>.Ok(new CestaResponse
        {
            CestaId = cesta.Id,
            Nome = cesta.Nome,
            Ativa = cesta.Ativa,
            DataCriacao = cesta.DataCriacao,
            Itens = cesta.ComposicaoCesta.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList()
        });
    }

    public async Task<Result<List<CestaResponse>>> Handle(CestaHistoricoRequest request, CancellationToken cancellationToken)
    {
        var cestas = await _cestaService.HistoricoCestasAsync(cancellationToken);

        if (cestas.Value is null)
            return Result<List<CestaResponse>>.Ok([]);

        return Result<List<CestaResponse>>.Ok(cestas.Value.Select(c => new CestaResponse
        {
            CestaId = c.Id,
            Nome = c.Nome,
            Ativa = c.Ativa,
            DataCriacao = c.DataCriacao,
            Itens = c.ComposicaoCesta.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList()
        }).ToList());
    }
}