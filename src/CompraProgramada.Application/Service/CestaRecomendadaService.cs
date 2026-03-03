using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class CestaRecomendadaService : ICestaRecomendadaService
{
    private readonly IClienteService _clienteService;
    private readonly ICestaRecomendadaRepository _cestaRepository;

    public CestaRecomendadaService(IClienteService clienteService,
        ICestaRecomendadaRepository cestaRepository)
    {
        _clienteService = clienteService;
        _cestaRepository = cestaRepository;
    }

    public async Task<Result<CriarAlterarCestaResponse>> CriarCestaAsync(CriarAlterarCestaRequest request, CancellationToken cancellationToken)
    {
        if (request.Itens.Count != 5)
            return Result<CriarAlterarCestaResponse>.Fail(new ErroMapeadoException($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {request.Itens.Count}.", "QUANTIDADE_ATIVOS_INVALIDA"));

        var somaPercentuais = (int)request.Itens.Sum(i => i.Percentual);

        if (somaPercentuais != 100)
            return Result<CriarAlterarCestaResponse>.Fail(new ErroMapeadoException($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {somaPercentuais}%.", "PERCENTUAIS_INVALIDOS"));

        var (cestaAnterior, atualizouCesta, usuariosAfetados) = await DesativaCestaAtualAsync(cancellationToken);

        var itensComposicaoCesta = request.Itens.Select(i => new ComposicaoCesta(0, 0, i.Ticker, i.Percentual)).ToList();

        var cestaCriada = await _cestaRepository.CriarAsync(new (0, request.Nome, DateTime.UtcNow, DateTime.UtcNow) { ComposicaoCesta = itensComposicaoCesta }, cancellationToken);

        List<string>? ativosRemovidos = null, ativosAdicionados = null;

        if (atualizouCesta)
            (ativosRemovidos, ativosAdicionados) = ObterMudancasDeAtivos(cestaAnterior.ComposicaoCesta, cestaAnterior.ComposicaoCesta).Value;

        var mensagemOperacao = !atualizouCesta ? "Primeira cesta cadastrada com sucesso." : $"Cesta atualizada. Rebalanceamento disparado para {usuariosAfetados} clientes ativos.";
        
        return Result<CriarAlterarCestaResponse>.Ok(new CriarAlterarCestaResponse
        {
            CestaId = cestaCriada.Id,
            Nome = cestaCriada.Nome,
            Ativa = cestaCriada.Ativa,
            DataCriacao = cestaCriada.DataCriacao,
            Itens = cestaCriada.ComposicaoCesta.Select(cc => new ComposicaoCestaDto(cc.Ticker, cc.Percentual)).ToList(),
            CestaAnteriorDesativada = cestaAnterior is null ? default : new CestaDesativadaDto { CestaId = cestaAnterior.Id, Nome = cestaAnterior.Nome, DataDesativacao = cestaAnterior.DataDesativacao },
            AtivosRemovidos = ativosRemovidos is not null ? ativosRemovidos : default,
            AtivosAdicionados = ativosAdicionados is not null ? ativosAdicionados : default,
            RebalanceamentoDisparado = !atualizouCesta ? false : true,
            Mensagem = mensagemOperacao
        });
    }

    public async Task<Result<CestaRecomendada?>> ObterCestaAtivaAsync(CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

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

    private async Task<(CestaRecomendada cestaAnterior, bool atualizouCesta, int usuariosAfetados)> DesativaCestaAtualAsync(CancellationToken cancellationToken)
    {
        var cestaAtual = await _cestaRepository.ObterCestaAtivaAsync(cancellationToken);

        if (cestaAtual is not null)
        {
            var cestaAnteriorAtualizada = cestaAtual with { Ativa = false, DataDesativacao = DateTime.UtcNow };

            await _cestaRepository.AtualizarAsync(cestaAtual, cestaAnteriorAtualizada, cancellationToken);
            
            var usuariosAfetados = await _clienteService.QuantidadeAtivosAsync(cancellationToken);

            return (cestaAnteriorAtualizada, true, usuariosAfetados.Value);
        }

        return (cestaAtual!, false, default);
    }
}