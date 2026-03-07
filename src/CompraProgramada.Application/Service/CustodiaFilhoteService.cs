using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CustodiaFilhoteService : ICustodiaFilhoteService
{
    private readonly ICustodiaFilhoteRepository _custodiaFilhoteRepository;
    private readonly ICotacaoService _cotacaoService;

    public CustodiaFilhoteService(ICustodiaFilhoteRepository custodiaFilhoteRepository,
        ICotacaoService cotacaoService)
    {
        _custodiaFilhoteRepository = custodiaFilhoteRepository;
        _cotacaoService = cotacaoService;
    }

    public async Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiaFilhoteContasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken)
    {
        if (!contas.Any())
            return new ApplicationException("Nenhuma conta gráfica informada para atualização.");

        var contasCustodias = contas.Select(c => new ContaGrafica
        (
            c.Id,
            c.NumeroConta,
            c.DataCriacao,
            c.ClienteId
        )
        {
            CustodiaFilhotes = c.CustodiaFilhotes!
                .Select(cf => new CustodiaFilhote(cf.Id, cf.ContaGraficaId, cf.Ticker, cf.PrecoMedio, cf.Quantidade)).ToList(),
            HistoricoComprar = c.HistoricoCompra!
                .Select(hc => new HistoricoCompra(hc.Id, hc.Data, hc.Valor, hc.ContaGraficaId)).ToList(),
        }).ToList();

        var custodias = contasCustodias.SelectMany(c => c.CustodiaFilhotes).ToList();

        var historicoCompra = contasCustodias.SelectMany(c => c.HistoricoComprar).ToList();

        var custodiasSalvas = await _custodiaFilhoteRepository.AtualizarCustodiasAsync(custodias, cancellationToken);

        return custodiasSalvas.Select(c => new CustodiaFilhoteDto(
            c.Id,
            c.ContaGraficaId,
            c.Ticker!,
            c.PrecoMedio,
            c.Quantidade
        )).ToList();
    }

    public async Task<Result<CarteiraDto>> ObterRentabilidadeDaCertira(List<CustodiaFilhoteDto> custodias, CancellationToken cancellationToken)
    {
        var cotacoesFechamentoCesta = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cancellationToken);
        if (!cotacoesFechamentoCesta.IsSuccess)
            return new ErroMapeadoException("Falha ao obter cotações do ativo na B3", "COTACOES_FECHAMENTO");

        decimal valorTotalInvestido = 0;
        decimal valorAtualCarteira = 0;
        decimal plTotal = 0;

        foreach (var custodia in custodias)
        {
            var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;

            var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
            var valorAtual = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;
            var plAtivo = (fechamentoAtivo.PrecoFechamento - custodia.PrecoMedio) * custodia.Quantidade;

            plTotal += plAtivo;
            valorTotalInvestido += valorInvestido;
            valorAtualCarteira += valorAtual;
        }

        var rentabilidadePercentual = ((valorAtualCarteira / valorTotalInvestido) - 1) * 100;

        var detalhesAtivos = custodias
            .Select(custodia =>
        {
            var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;
            var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
            var valorAtual = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;
            var pl = (fechamentoAtivo.PrecoFechamento - custodia.PrecoMedio) * custodia.Quantidade;
            var plPercentual = valorAtual < 1 ? 0 : ((valorAtual / valorInvestido) - 1) * 100;
            var composicaoCarteira = valorAtual < 1 ? 0 : (valorAtual / valorAtualCarteira) * 100;

            return new DetalheAtivoCarteiraDto(
                custodia.Ticker,
                custodia.Quantidade,
                custodia.PrecoMedio,
                fechamentoAtivo.PrecoFechamento,
                valorAtual,
                pl,
                Math.Round(plPercentual, 2),
                Math.Round(composicaoCarteira, 2));
        }).ToList();

        return new CarteiraDto(
            new ResumoCarteiraDto(valorTotalInvestido, valorAtualCarteira, plTotal, Math.Round(rentabilidadePercentual, 2)), detalhesAtivos);
    }
}