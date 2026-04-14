using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Handler;

public class CriarCestaHandler : IRequestHandler<CriarCestaRecomendadaRequest, Result<CriarCestaRecomendadaResponse>>
{
    private readonly ILogger<CriarCestaHandler> _logger;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly IClienteService _clienteService;

    public CriarCestaHandler(ILogger<CriarCestaHandler> logger,
        ICestaRecomendadaService cestaService,
        IClienteService clienteService)
    {
        _logger = logger;
        _cestaService = cestaService;
        _clienteService = clienteService;
    }

    public async Task<Result<CriarCestaRecomendadaResponse>> Handle(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de criação de cesta: {Request}", request);

        var result = await _cestaService.CriarCestaAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao processar criação da cesta: {Exception}", result.Exception);
            return result.Exception;
        }

        var cestaAtual = result.Value.CestaAtual;
        var cestaAnterior = result.Value.CestaAnterior;
        var teveCestaAtualizada = result.Value.CestaAtualizada;
        var response = MontarResponseCriarAlterarCesta(cestaAtual, false, cestaAnterior, default, default, "Primeira cesta cadastrada com sucesso.");

        if (!teveCestaAtualizada)
        {
            _logger.LogInformation("Cesta criada com sucesso!");
            return response;
        }

        _logger.LogInformation("Uma cesta foi desativada para uma nova ser ativada. Nova cesta: {NovaCesta}", result.Value.CestaAtual);

        var quantidadeClientesAtivosResult = await _clienteService.QuantidadeClientesAtivosAsync(cancellationToken);
        if (!quantidadeClientesAtivosResult.IsSuccess)
            return quantidadeClientesAtivosResult.Exception;

        var (ativosRemovidos, ativosAdicionados) = _cestaService.ObterMudancasDeAtivos(cestaAnterior!.Itens, cestaAtual.Itens);

        var mensagemOperacao = $"Cesta atualizada. Rebalanceamento disparado para {quantidadeClientesAtivosResult.Value} clientes ativos.";

        return response with
        {
            RebalanceamentoDisparado = true,
            AtivosRemovidos = ativosRemovidos,
            AtivosAdicionados = ativosAdicionados,
            Mensagem = mensagemOperacao
        };
    }

    private CriarCestaRecomendadaResponse MontarResponseCriarAlterarCesta(CestaRecomendadaDto cesta, bool atualizouCesta, CestaRecomendadaDto? cestaAnterior, List<string>? ativosRemovidos, List<string>? ativosAdicionados, string mensagemOperacao)
        => new CriarCestaRecomendadaResponse(
            cesta.CestaId,
            cesta.Nome,
            cesta.Ativa,
            cesta.DataCriacao,
            cesta.Itens.Select(cc => new ComposicaoCestaDto { Ticker = cc.Ticker, Percentual = cc.Percentual }).ToList(),
            cestaAnterior is null ? default : new CestaDesativadaDto { CestaId = cestaAnterior.CestaId, Nome = cestaAnterior.Nome, DataDesativacao = cestaAnterior.DataDesativacao!.Value },
            ativosRemovidos,
            ativosAdicionados,
            atualizouCesta,
            mensagemOperacao
        );
}