using CompraProgramada.Domain.Contract.Handler;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Domain.Handler.Api;

public class AdministradorHandler
    : IRequestHandler<CriarCestaRecomendadaRequest, Result<CriarCestaRecomendadaResponse>>,
        IRequestHandler<CestaAtualRequest, Result<CestaRecomendadaDto>>,
        IRequestHandler<CestaHistoricoRequest, Result<HistoricoCestasResponse>>, IApiRequestHandler
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly CestaRecomendadaMapper _mapper;

    public AdministradorHandler(ILogger<AdministradorHandler> logger,
        ICestaRecomendadaRepository cestaRecomendadaRepository,
        IClienteRepository clienteRepository,
        CestaRecomendadaMapper mapper)
    {
        _logger = logger;
        _cestaRecomendadaRepository = cestaRecomendadaRepository;
        _clienteRepository = clienteRepository;
        _mapper = mapper;
    }

    public async Task<Result<CriarCestaRecomendadaResponse>> Handle(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de criação/atualização de cesta: {Request}", request);

        var cestaAnteriorResult = await ObtemEDesativaCestaAtual(cancellationToken);
        if (!cestaAnteriorResult.IsSuccess)
            return cestaAnteriorResult.Exception;

        var cestaAnterior = cestaAnteriorResult.Value;

        var itensCesta = request.Itens.Select(i => ComposicaoCesta.CriaItemNaCesta(i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRecomendadaRepository.CriarAsync(CestaRecomendada.CriarCesta(request.Nome, itensCesta), cancellationToken);

        _logger.LogInformation("Cesta registrada na base de dados {Cesta}", cestaCriada);

        var cestaAtualizada = cestaAnterior is not null;

        if (!cestaAtualizada)
            return ResponseCriarAlterarCesta(cestaCriada, false, default, default, default);

        var quantidadeUsuariosAtivos = await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);

        var (ativosRemovidos, ativosAdicionados) = ObterMudancasDeAtivos(cestaAnterior!.ComposicaoCesta, cestaCriada.ComposicaoCesta);

        var msgOperacaoComAtualizacao = $"Cesta atualizada. Rebalanceamento disparado para {quantidadeUsuariosAtivos} clientes ativos.";
        return ResponseCriarAlterarCesta(cestaCriada, true, cestaAnterior, ativosRemovidos, ativosAdicionados) with { Mensagem = msgOperacaoComAtualizacao };
    }

    public async Task<Result<CestaRecomendadaDto>> Handle(CestaAtualRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando consulta da cesta atual.");

        var cesta = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);

        if (cesta is null)
            return new ApplicationException("Nenhuma Cesta Top Five ativa no momento.");

        // Obter fechamento atual para retornar

        return _mapper.ToResponse(cesta);
    }

    public async Task<Result<HistoricoCestasResponse>> Handle(CestaHistoricoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de consulta de histórico de cestas.");

        var cestas = await _cestaRecomendadaRepository.ObterCestasAsync(cancellationToken);

        var cestasDto = cestas.Select(x => _mapper.ToResponse(x)).ToList();

        return new HistoricoCestasResponse(cestasDto);
    }

    private CriarCestaRecomendadaResponse ResponseCriarAlterarCesta(CestaRecomendada cesta, bool atualizouCesta, CestaRecomendada? cestaAnterior, List<string>? ativosRemovidos, List<string>? ativosAdicionados)
        => new CriarCestaRecomendadaResponse(
            cesta.Id,
            cesta.Nome,
            cesta.Ativa,
            cesta.DataCriacao,
            cesta.ComposicaoCesta.Select(cc => new ComposicaoCestaDto { Ticker = cc.Ticker, Percentual = cc.Percentual }).ToList(),
            cestaAnterior is null ? default : new CestaDesativadaDto { CestaId = cestaAnterior!.Id, Nome = cestaAnterior.Nome, DataDesativacao = cestaAnterior.DataDesativacao!.Value },
            ativosRemovidos,
            ativosAdicionados,
            atualizouCesta
        );

    private async Task<Result<CestaRecomendada?>> ObtemEDesativaCestaAtual(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);

        if (cestaAtual is null)
            return Result.Success<CestaRecomendada>(default!)!;

        cestaAtual.DesativarCesta();

        await _cestaRecomendadaRepository.AtualizarAsync(cestaAtual, cancellationToken);

        _logger.LogInformation("Cesta atual desativada {Cesta}", cestaAtual);

        return cestaAtual;
    }

    internal (List<string> ativosRemovidos, List<string> ativosAdicionados) ObterMudancasDeAtivos(List<ComposicaoCesta> composicaoAnterior, List<ComposicaoCesta> composicaoAtual)
    {
        var tickersAnteriores = composicaoAnterior.Select(c => c.Ticker);
        var tickersAtual = composicaoAtual.Select(c => c.Ticker);

        var ativosRemovidos = tickersAnteriores
            .Except(tickersAtual)
            .ToList();

        var ativosAdicionados = tickersAtual
            .Except(tickersAnteriores)
            .ToList();

        _logger.LogInformation("Mudanças de ativos identificados {Removidos} - {Adicionados}", ativosRemovidos, ativosAdicionados);

        return (ativosRemovidos, ativosAdicionados);
    }
}