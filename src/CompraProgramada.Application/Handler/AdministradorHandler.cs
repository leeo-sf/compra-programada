using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Handler;

public class AdministradorHandler
    : IRequestHandler<CriarAlterarCestaRequest, Result<CriarAlterarCestaResponse>>,
        IRequestHandler<CestaAtualRequest, Result<CestaResponse>>,
        IRequestHandler<CestaHistoricoRequest, Result<List<CestaResponse>>>
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly IClienteService _clienteService;
    private const string PRIMEIRA_CESTA_CADASTRADA_MENSAGEM = "Primeira cesta cadastrada com sucesso.";

    public AdministradorHandler(ILogger<AdministradorHandler> logger,
        IClienteService clienteService,
        ICestaRecomendadaService cestaService)
    {
        _logger = logger;
        _clienteService = clienteService;
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

        if (!result.Value!.CestaAtualizada)
            return Result<CriarAlterarCestaResponse>.Ok(MontarResponseCriarAlterarCesta(result.Value.CestaAtual, result.Value.CestaAtualizada, default, default, default, PRIMEIRA_CESTA_CADASTRADA_MENSAGEM));

        var usuariosAtivos = await _clienteService.QuantidadeAtivosAsync(cancellationToken);

        if (!usuariosAtivos.IsSuccess)
            return Result<CriarAlterarCestaResponse>.Fail(new ApplicationException("Falha ao obter quantidade de clientes ativos."));

        var (ativosRemovidos, ativosAdicionados) = _cestaService.ObterMudancasDeAtivos(result.Value.CestaAnterior!.ComposicaoCesta, result.Value.CestaAtual.ComposicaoCesta).Value;

        var mensagemOperacao = $"Cesta atualizada. Rebalanceamento disparado para {usuariosAtivos.Value} clientes ativos.";
        return Result<CriarAlterarCestaResponse>.Ok(
            MontarResponseCriarAlterarCesta(result.Value.CestaAtual, result.Value.CestaAtualizada, result.Value.CestaAnterior, ativosRemovidos, ativosAdicionados, mensagemOperacao));
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

    private CriarAlterarCestaResponse MontarResponseCriarAlterarCesta(CestaRecomendada cesta, bool atualizouCesta, CestaRecomendada? cestaAnterior, List<string>? ativosRemovidos, List<string>? ativosAdicionados, string mensagemOperacao)
        => new CriarAlterarCestaResponse
        {
            CestaId = cesta.Id,
            Nome = cesta.Nome,
            Ativa = cesta.Ativa,
            DataCriacao = cesta.DataCriacao,
            Itens = cesta.ComposicaoCesta.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList(),
            CestaAnteriorDesativada = cestaAnterior is null ? default : new CestaDesativadaDto { CestaId = cestaAnterior!.Id, Nome = cestaAnterior.Nome, DataDesativacao = cestaAnterior.DataDesativacao },
            AtivosRemovidos = ativosRemovidos is not null ? ativosRemovidos : default,
            AtivosAdicionados = ativosAdicionados is not null ? ativosAdicionados : default,
            RebalanceamentoDisparado = !atualizouCesta ? false : true,
            Mensagem = mensagemOperacao
        };
}