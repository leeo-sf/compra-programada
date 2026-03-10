using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Handler;

public class AdministradorHandler
    : IRequestHandler<CriarAlterarCestaRequest, Result<CriarAlterarCestaResponse>>,
        IRequestHandler<CestaAtualRequest, Result<CestaRecomendadaResponse>>,
        IRequestHandler<CestaHistoricoRequest, Result<List<CestaRecomendadaResponse>>>
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
            return result.Exception;
        }

        if (!result.Value!.CestaAtualizada)
            return MontarResponseCriarAlterarCesta(result.Value.CestaAtual, result.Value.CestaAtualizada, default, default, default, PRIMEIRA_CESTA_CADASTRADA_MENSAGEM);
        
        _logger.LogInformation("Cesta criada com sucesso!");

        var quantidadeUsuariosAtivos = await _clienteService.QuantidadeAtivosAsync(cancellationToken);
        if (!quantidadeUsuariosAtivos.IsSuccess)
            return quantidadeUsuariosAtivos.Exception;

        var (ativosRemovidos, ativosAdicionados) = _cestaService.ObterMudancasDeAtivos(result.Value.CestaAnterior!.Itens, result.Value.CestaAtual.Itens).Value;

        var mensagemOperacao = $"Cesta atualizada. Rebalanceamento disparado para {quantidadeUsuariosAtivos.Value} clientes ativos.";
        return MontarResponseCriarAlterarCesta(result.Value.CestaAtual, result.Value.CestaAtualizada, result.Value.CestaAnterior, ativosRemovidos, ativosAdicionados, mensagemOperacao);
    }

    public async Task<Result<CestaRecomendadaResponse>> Handle(CestaAtualRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando consulta da cesta atual.");

        var result = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (!result.IsSuccess)
            return result.Exception;

        var cesta = result.Value!;

        return new CestaRecomendadaResponse
        {
            CestaId = cesta.Id,
            Nome = cesta.Nome,
            Ativa = cesta.Ativa,
            DataCriacao = cesta.DataCriacao,
            Itens = cesta.Itens.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList()
        };
    }

    public async Task<Result<List<CestaRecomendadaResponse>>> Handle(CestaHistoricoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de consulta de histórico de cestas.");

        var cestas = await _cestaService.HistoricoCestasAsync(cancellationToken);

        if (cestas.Value is null)
            return new List<CestaRecomendadaResponse>();

        return cestas.Value.Select(c => new CestaRecomendadaResponse
        {
            CestaId = c.Id,
            Nome = c.Nome,
            Ativa = c.Ativa,
            DataCriacao = c.DataCriacao,
            Itens = c.Itens.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList()
        }).ToList();
    }

    private CriarAlterarCestaResponse MontarResponseCriarAlterarCesta(CestaRecomandadaDto cesta, bool atualizouCesta, CestaRecomandadaDto? cestaAnterior, List<string>? ativosRemovidos, List<string>? ativosAdicionados, string mensagemOperacao)
        => new CriarAlterarCestaResponse
        {
            CestaId = cesta.Id,
            Nome = cesta.Nome,
            Ativa = cesta.Ativa,
            DataCriacao = cesta.DataCriacao,
            Itens = cesta.Itens.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList(),
            CestaAnteriorDesativada = cestaAnterior is null ? default : new CestaDesativadaDto(cestaAnterior!.Id, cestaAnterior.Nome, cestaAnterior.DataDesativacao!.Value),
            AtivosRemovidos = ativosRemovidos is not null ? ativosRemovidos : default,
            AtivosAdicionados = ativosAdicionados is not null ? ativosAdicionados : default,
            RebalanceamentoDisparado = !atualizouCesta ? false : true,
            Mensagem = mensagemOperacao
        };
}