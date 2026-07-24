using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Domain.Handler.Api;

public class CriarCestaHandler : IRequestHandler<CriarCestaRecomendadaRequest, Result<CriarCestaRecomendadaResponse>>
{
    private readonly ILogger<CriarCestaHandler> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly IClienteRepository _clienteRepository;

    public CriarCestaHandler(
        ILogger<CriarCestaHandler> logger,
        ICestaRecomendadaRepository cestaRecomendadaRepository,
        IClienteRepository clienteRepository)
    {
        _logger = logger;
        _cestaRecomendadaRepository = cestaRecomendadaRepository;
        _clienteRepository = clienteRepository;
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

        var (ativosRemovidos, ativosAdicionados) = ObterMudancasDeAtivos([.. cestaAnterior!.ComposicaoCesta.Select(c => c.Ticker)], [.. cestaCriada.ComposicaoCesta.Select(c => c.Ticker)]);

        var msgOperacaoComAtualizacao = $"Cesta atualizada. Rebalanceamento disparado para {quantidadeUsuariosAtivos} clientes ativos.";
        return ResponseCriarAlterarCesta(cestaCriada, true, cestaAnterior, ativosRemovidos, ativosAdicionados) with { Mensagem = msgOperacaoComAtualizacao };
    }

    private static CriarCestaRecomendadaResponse ResponseCriarAlterarCesta(CestaRecomendada cesta, bool atualizouCesta, CestaRecomendada? cestaAnterior, List<string>? ativosRemovidos, List<string>? ativosAdicionados)
        => new(
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
        var cesta = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);

        if (cesta is null)
            return Result.Success<CestaRecomendada>(default!)!;

        cesta.DesativarCesta();

        await _cestaRecomendadaRepository.AtualizarAsync(cesta, cancellationToken);

        _logger.LogInformation("Cesta atual desativada {Cesta}", cesta);

        return cesta;
    }

    internal static (List<string> ativosRemovidos, List<string> ativosAdicionados) ObterMudancasDeAtivos(List<string> tickersAnteriores, List<string> tickersAtuais)
    {
        var ativosRemovidos = tickersAnteriores
            .Except(tickersAtuais)
            .ToList();

        var ativosAdicionados = tickersAtuais
            .Except(tickersAnteriores)
            .ToList();

        return (ativosRemovidos, ativosAdicionados);
    }
}