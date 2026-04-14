using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Shared.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.Extensions.Logging;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Service;

public class CestaRecomendadaService : ICestaRecomendadaService
{
    private readonly ILogger<CestaRecomendadaService> _logger;
    private readonly ICestaRecomendadaRepository _cestaRepository;
    private readonly CestaRecomendadaMapper _mapper;

    public CestaRecomendadaService(ILogger<CestaRecomendadaService> logger,
        ICestaRecomendadaRepository cestaRepository,
        CestaRecomendadaMapper mapper)
    {
        _logger = logger;
        _cestaRepository = cestaRepository;
        _mapper = mapper;
    }

    public async Task<Result<CriarCestaRecomendadaDto>> CriarCestaAsync(CriarCestaRecomendadaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de registro de uma nova cesta...");

        var cestaAnterior = await DesativaCestaAtualAsync(cancellationToken);

        var itensCesta = request.Itens.Select(i => ComposicaoCesta.CriaItemNaCesta(i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRepository.CriarAsync(CestaRecomendada.CriarCesta(request.Nome, itensCesta), cancellationToken);

        _logger.LogInformation("Cesta registrada na base de dados {Cesta}", cestaCriada);

        return new CriarCestaRecomendadaDto
        {
            CestaAtualizada = cestaAnterior is not null ? true : false,
            CestaAtual = _mapper.ToResponse(cestaCriada),
            CestaAnterior = cestaAnterior is not null ? _mapper.ToResponse(cestaAnterior) : null
        };
    }

    public async Task<Result<CestaRecomendada?>> ObterCestaAtivaAsync(CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cesta is null)
            return new ApplicationException("Nenhuma Cesta Top Five ativa no momento.");

        return cesta;
    }

    public async Task<Result<List<CestaRecomendada>>> HistoricoCestasAsync(CancellationToken cancellationToken)
    {
        var cestas = await _cestaRepository.ObterTodasCestasAsync(cancellationToken);

        if (cestas is not null)
            return cestas;

        return Result.Success<List<CestaRecomendada>>([]);
    }

    public async Task<Result<List<ValorAtivoConsolidadoDto>>> ValorPorAtivoConsolidado(decimal totalConsolidado, CancellationToken cancellationToken)
    {
        var cestaVigente = await ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var valorPorAtivoConsolidado = cestaVigente.Value.ComposicaoCesta
            .Select(ativo => new ValorAtivoConsolidadoDto { Ticker = ativo.Ticker, ValorDeCompraAtivo = totalConsolidado * (ativo.Percentual / 100) })
            .ToList();

        return valorPorAtivoConsolidado;
    }

    public (List<string> ativosRemovidos, List<string> ativosAdicionados) ObterMudancasDeAtivos(List<ComposicaoCestaDto> composicaoAnterior, List<ComposicaoCestaDto> composicaoAtual)
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

    private async Task<CestaRecomendada?> DesativaCestaAtualAsync(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cestaAtual is not null)
        {
            cestaAtual.DesativarCesta();

            await _cestaRepository.AtualizarAsync(cestaAtual, cancellationToken);

            _logger.LogInformation("Cesta atual desativada {Cesta}", cestaAtual);

            return cestaAtual;
        }

        _logger.LogInformation("Nenhuma cesta foi encontrada na base de dados para desativação");

        return null;
    }
}