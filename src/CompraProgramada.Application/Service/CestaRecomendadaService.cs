using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CestaRecomendadaService : ICestaRecomendadaService
{
    private readonly ICestaRecomendadaRepository _cestaRepository;

    public CestaRecomendadaService(ICestaRecomendadaRepository cestaRepository) => _cestaRepository = cestaRepository;

    public async Task<Result<CriarAlterarCestaDto>> CriarCestaAsync(CriarAlterarCestaRequest request, CancellationToken cancellationToken)
    {
        if (request.Itens.Count != 5)
            return new ErroMapeadoException($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {request.Itens.Count}.", "QUANTIDADE_ATIVOS_INVALIDA");

        var somaPercentuais = (int)request.Itens.Sum(i => i.Percentual);

        if (somaPercentuais != 100)
            return new ErroMapeadoException($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {somaPercentuais}%.", "PERCENTUAIS_INVALIDOS");

        var (cestaAnterior, atualizouCesta) = await DesativaCestaAtualAsync(cancellationToken);

        var itensComposicaoCesta = request.Itens.Select(i => new ComposicaoCesta(0, 0, i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRepository.CriarAsync(new (0, request.Nome, DateTime.UtcNow, DateTime.UtcNow) { ComposicaoCesta = itensComposicaoCesta }, cancellationToken);

        var resposta = new CriarAlterarCestaDto
        {
            CestaAtualizada = atualizouCesta,
            CestaAtual = GerarCestaDto(cestaCriada),
            CestaAnterior = atualizouCesta ? cestaAnterior : null
        };

        return resposta;
    }

    public async Task<Result<CestaRecomandadaDto?>> ObterCestaAtivaAsync(CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cesta is null)
            return new ApplicationException("Cesta Top Five não cadastrada.");

        return GerarCestaDto(cesta);
    }

    public async Task<Result<IEnumerable<CestaRecomandadaDto>>> HistoricoCestasAsync(CancellationToken cancellationToken)
    {
        var cestas = await _cestaRepository.ObterTodasCestasAsync(cancellationToken);

        return cestas.Select(c => GerarCestaDto(c))
            .OrderByDescending(c => c.Id)
            .ToList();
    }

    public Result<(List<string> ativosRemovidos, List<string> ativosAdicionados)> ObterMudancasDeAtivos(List<ComposicaoCestaRecomendadaDto> composicaoAnterior, List<ComposicaoCestaRecomendadaDto> composicaoAtual)
    {
        var tickersAnteriores = composicaoAnterior.Select(c => c.Ticker);
        var tickersAtual = composicaoAtual.Select(c => c.Ticker);

        var ativosRemovidos = tickersAnteriores
            .Except(tickersAtual)
            .ToList();

        var ativosAdicionados = tickersAtual
            .Except(tickersAnteriores)
            .ToList();

        return (ativosRemovidos, ativosAdicionados);
    }

    private async Task<(CestaRecomandadaDto cestaAnterior, bool atualizouCesta)> DesativaCestaAtualAsync(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cestaAtual is not null)
        {
            var cestaAnteriorAtualizada = cestaAtual with { Ativa = false, DataDesativacao = DateTime.Now };

            await _cestaRepository.AtualizarAsync(cestaAtual, cestaAnteriorAtualizada, cancellationToken);

            return (GerarCestaDto(cestaAnteriorAtualizada), true);
        }

        return (GerarCestaDto(cestaAtual!), false);
    }

    public Result<List<ValorAtivoConsolidadoDto>> ValorPorAtivoConsolidado(CestaRecomandadaDto cesta, decimal totalConsolidado)
    {
        var valorPorAtivoConsolidado = cesta.Itens
            .Select(ativo => new ValorAtivoConsolidadoDto { Ticker = ativo.Ticker, ValorDeCompraPorAtivo = totalConsolidado * (ativo.Percentual / 100) })
            .ToList();

        return valorPorAtivoConsolidado;
    }

    private CestaRecomandadaDto GerarCestaDto(CestaRecomendada cesta)
        => new CestaRecomandadaDto
        {
            Id = cesta.Id,
            Nome = cesta.Nome,
            DataCriacao = cesta.DataCriacao,
            DataDesativacao = cesta.DataDesativacao,
            Ativa = cesta.Ativa,
            Itens = cesta.ComposicaoCesta.Select(cc => new ComposicaoCestaRecomendadaDto
            {
                Id = cc.Id,
                CestaId = cc.Id,
                Ticker = cc.Ticker,
                Percentual = cc.Percentual
            }).ToList()
        };
}