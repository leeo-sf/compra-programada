using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class CestaRecomendadaService : ICestaRecomendadaService
{
    private readonly ICestaRecomendadaRepository _cestaRepository;

    public CestaRecomendadaService(ICestaRecomendadaRepository cestaRepository) => _cestaRepository = cestaRepository;

    public async Task<Result<CriarAlterarCestaDto>> CriarCestaAsync(CriarAlterarCestaRequest request, CancellationToken cancellationToken)
    {
        if (request.Itens.Count != 5)
            return Result<CriarAlterarCestaDto>.Fail(new ErroMapeadoException($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {request.Itens.Count}.", "QUANTIDADE_ATIVOS_INVALIDA"));

        var somaPercentuais = (int)request.Itens.Sum(i => i.Percentual);

        if (somaPercentuais != 100)
            return Result<CriarAlterarCestaDto>.Fail(new ErroMapeadoException($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {somaPercentuais}%.", "PERCENTUAIS_INVALIDOS"));

        var (cestaAnterior, atualizouCesta) = await DesativaCestaAtualAsync(cancellationToken);

        var itensComposicaoCesta = request.Itens.Select(i => new ComposicaoCesta(0, 0, i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRepository.CriarAsync(new (0, request.Nome, DateTime.UtcNow, DateTime.UtcNow) { ComposicaoCesta = itensComposicaoCesta }, cancellationToken);

        var resposta = new CriarAlterarCestaDto
        {
            CestaAtualizada = atualizouCesta,
            CestaAtual = cestaCriada,
            CestaAnterior = cestaAnterior is not null ? cestaAnterior : null
        };

        return Result<CriarAlterarCestaDto>.Ok(resposta);
    }

    public async Task<Result<CestaRecomendada?>> ObterCestaAtivaAsync(CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cesta is null)
            return Result<CestaRecomendada?>.Fail(new ApplicationException("Cesta Top Five não cadastrada."));

        return Result<CestaRecomendada?>.Ok(cesta);
    }

    public async Task<Result<IEnumerable<CestaRecomendada>>> HistoricoCestasAsync(CancellationToken cancellationToken)
    {
        var cestas = await _cestaRepository.ObterTodasCestasAsync(cancellationToken);

        return Result<IEnumerable<CestaRecomendada>>.Ok(cestas.OrderByDescending(x => x.Id));
    }

    public Result<(List<string> ativosRemovidos, List<string> ativosAdicionados)> ObterMudancasDeAtivos(List<ComposicaoCesta> composicaoAnterior, List<ComposicaoCesta> composicaoAtual)
    {
        var tickersAnteriores = composicaoAnterior.Select(c => c.Ticker);
        var tickersAtual = composicaoAtual.Select(c => c.Ticker);

        var ativosRemovidos = tickersAnteriores
            .Except(tickersAtual)
            .ToList();

        var ativosAdicionados = tickersAtual
            .Except(tickersAnteriores)
            .ToList();

        return Result<(List<string>, List<string>)>.Ok((ativosRemovidos, ativosAdicionados));
    }

    private async Task<(CestaRecomendada cestaAnterior, bool atualizouCesta)> DesativaCestaAtualAsync(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cestaAtual is not null)
        {
            var cestaAnteriorAtualizada = cestaAtual with { Ativa = false, DataDesativacao = DateTime.UtcNow };

            await _cestaRepository.AtualizarAsync(cestaAtual, cestaAnteriorAtualizada, cancellationToken);

            return (cestaAnteriorAtualizada, true);
        }

        return (cestaAtual!, false);
    }
}