using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Confluent.Kafka;
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
            HistoricoCompra = c.HistoricoCompra!
                .Select(hc => HistoricoCompra.RegistrarHistorico(c.Id, hc.Ticker, hc.Quantidade, hc.PrecoExecutado, hc.PrecoMedio, hc.ValorAporte, hc.Data)).ToList(),
        }).ToList();

        var custodias = contasCustodias.SelectMany(c => c.CustodiaFilhotes).ToList();

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
        decimal valorTotalAtualCarteira = 0;
        decimal plTotal = 0;

        foreach (var custodia in custodias)
        {
            var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;

            var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
            var valorAtual = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;
            var plAtivo = (fechamentoAtivo.PrecoFechamento - custodia.PrecoMedio) * custodia.Quantidade;

            plTotal += plAtivo;
            valorTotalInvestido += valorInvestido;
            valorTotalAtualCarteira += valorAtual;
        }

        var rentabilidadePercentual = ((valorTotalAtualCarteira / valorTotalInvestido) - 1) * 100;

        var resumo = new ResumoCarteiraDto(valorTotalInvestido, valorTotalAtualCarteira, plTotal, Math.Round(rentabilidadePercentual, 2));

        var detalhesResult = ObterDetalhesDaCerteira(custodias, cotacoesFechamentoCesta.Value, valorTotalAtualCarteira);

        return new CarteiraDto(resumo, detalhesResult);
    }

    public async Task<Result<RentabilidadeDto>> ObterEvolucaoDaCertira(ContaGraficaDto conta, CancellationToken cancellationToken)
    {
        if (conta.HistoricoCompra is null || !conta.HistoricoCompra.Any())
            throw new ApplicationException("Cliente ainda não tem compras realizadas.");

        var cotacoesFechamentoCesta = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cancellationToken);
        if (!cotacoesFechamentoCesta.IsSuccess)
            return new ErroMapeadoException("Falha ao obter cotações do ativo na B3", "COTACOES_FECHAMENTO");

        var historicoCompra = conta.HistoricoCompra!
            .DistinctBy(d => d.Data)
            .OrderBy(g => g.Data)
            .Select((t, index) => new HistoricoAporteDto(
                0,
                t.Data,
                t.ValorAporte,
                $"{index + 1}/{conta.HistoricoCompra!.Select(x => x.Data).Distinct().Count()}"
            )).ToList();

        var evolucaoCarteira = conta.HistoricoCompra
            .DistinctBy(x => x.Data)
            .OrderBy(x => x.Data)
            .Select(x =>
            {
                var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == x.Ticker)!;

                var valorCarteira = x.Quantidade < 1 ? 0 : x.Quantidade * fechamentoAtivo.PrecoFechamento;
                var valorInvestido = x.Quantidade < 1 ? 0 : x.Quantidade * x.ValorAporte;
                var rentabilidade = x.Quantidade < 1 ? 0 : ((valorCarteira / valorInvestido) - 1) * 100;

                return new EvolucaoCarteiraDto(0, x.Data, valorCarteira, valorInvestido, rentabilidade);
            }).ToList();

        return new RentabilidadeDto(historicoCompra, evolucaoCarteira);
    }

    private List<DetalheAtivoCarteiraDto> ObterDetalhesDaCerteira(List<CustodiaFilhoteDto> custodias, CotacaoDto cotacoesFechamentoCesta, decimal valorTotalCarteira)
    {
        var detalhesAtivos = custodias
            .Select(custodia =>
            {
                var fechamentoAtivo = cotacoesFechamentoCesta.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;

                var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
                var valorAtual = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;
                var pl = (fechamentoAtivo.PrecoFechamento - custodia.PrecoMedio) * custodia.Quantidade;
                var plPercentual = valorAtual < 1 ? 0 : ((valorAtual / valorInvestido) - 1) * 100;
                var composicaoCarteira = valorAtual < 1 ? 0 : (valorAtual / valorTotalCarteira) * 100;

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

        return detalhesAtivos;
    }
}