using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Handler;

public class AdministradorHandler
    : IRequestHandler<CriarCestaRecomendadaRequest, Result<CriarCestaRecomendadaResponse>>,
        IRequestHandler<CestaAtualRequest, Result<CestaRecomendadaResponse>>,
        IRequestHandler<CestaHistoricoRequest, Result<List<CestaRecomendadaResponse>>>
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly IClienteService _clienteService;
    private readonly CestaRecomendadaMapper _mapper;

    public AdministradorHandler(ILogger<AdministradorHandler> logger,
        IClienteService clienteService,
        ICestaRecomendadaService cestaService,
        CestaRecomendadaMapper mapper)
    {
        _logger = logger;
        _clienteService = clienteService;
        _cestaService = cestaService;
        _mapper = mapper;
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

        if (!result.Value!.CestaAtualizada)
        {
            _logger.LogInformation("Cesta criada com sucesso!");
            return MontarResponseCriarAlterarCesta(result.Value.CestaAtual, result.Value.CestaAtualizada, default, default, default);
        }

        _logger.LogInformation("Cesta alterada com sucesso!");

        var quantidadeUsuariosAtivos = await _clienteService.QuantidadeClientesAtivosAsync(cancellationToken);
        if (!quantidadeUsuariosAtivos.IsSuccess)
            return quantidadeUsuariosAtivos.Exception;

        var (ativosRemovidos, ativosAdicionados) = _cestaService.ObterMudancasDeAtivos(result.Value.CestaAnterior!.Itens, result.Value.CestaAtual.Itens).Value;

        var mensagemOperacao = $"Cesta atualizada. Rebalanceamento disparado para {quantidadeUsuariosAtivos.Value} clientes ativos.";
        return MontarResponseCriarAlterarCesta(result.Value.CestaAtual, result.Value.CestaAtualizada, result.Value.CestaAnterior, ativosRemovidos, ativosAdicionados) with { Mensagem = mensagemOperacao };
    }

    public async Task<Result<CestaRecomendadaResponse>> Handle(CestaAtualRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando consulta da cesta atual.");

        var result = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (!result.IsSuccess)
            return result.Exception;

        var cesta = result.Value!;

        return _mapper.ToResponse(cesta);
    }

    public async Task<Result<List<CestaRecomendadaResponse>>> Handle(CestaHistoricoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de consulta de histórico de cestas.");

        var cestas = await _cestaService.HistoricoCestasAsync(cancellationToken);

        if (cestas.Value is null)
            return new List<CestaRecomendadaResponse>();

        return _mapper.ToResponse(cestas.Value);
    }

    private CriarCestaRecomendadaResponse MontarResponseCriarAlterarCesta(CestaRecomandadaDto cesta, bool atualizouCesta, CestaRecomandadaDto? cestaAnterior, List<string>? ativosRemovidos, List<string>? ativosAdicionados)
        => new CriarCestaRecomendadaResponse(
            cesta.Id,
            cesta.Nome,
            cesta.Ativa,
            cesta.DataCriacao,
            cesta.Itens.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList(),
            cestaAnterior is null ? default : new CestaDesativadaDto(cestaAnterior!.Id, cestaAnterior.Nome, cestaAnterior.DataDesativacao!.Value),
            ativosRemovidos is not null ? ativosRemovidos : default,
            ativosAdicionados is not null ? ativosAdicionados : default,
            !atualizouCesta ? false : default
        );
}