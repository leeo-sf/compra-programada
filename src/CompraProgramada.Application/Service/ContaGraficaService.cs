using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ContaGraficaService : IContaGraficaService
{
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly IContaGraficaRepository _contaGraficaRepository;
    private readonly ICotacaoService _cotacaoService;

    public ContaGraficaService(ICestaRecomendadaService cestaRecomendadaService,
        IContaGraficaRepository contaGraficaRepository,
        ICotacaoService cotacaoService)
    {
        _cestaRecomendadaService = cestaRecomendadaService;
        _contaGraficaRepository = contaGraficaRepository;
        _cotacaoService = cotacaoService;
    }

    public async Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var custodiasConta = cestaVigente.Value!.ComposicaoCesta
            .Select(x => CustodiaFilhote.GerarCustodia(x.Ticker)).ToList();

        var conta = ContaGrafica.Gerar(clienteId, custodiasConta);

        var contaSalva = await _contaGraficaRepository.CriarAsync(conta, cancellationToken);

        return new ContaGraficaDto(
            contaSalva.Id,
            contaSalva.NumeroConta,
            contaSalva.DataCriacao,
            contaSalva.ClienteId,
            contaSalva.Tipo,
            null,
            contaSalva.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto(
                cf.Id,
                cf.ContaGraficaId,
                cf.Ticker,
                cf.PrecoMedio,
                cf.Quantidade
            )).ToList()
        );
    }

    public async Task<Result<List<ContaGrafica>>> AtualizarContasAsync(List<ContaGrafica> contasAhSeremAtualizadas, CancellationToken cancellationToken)
    {
        if (!contasAhSeremAtualizadas.Any())
            return new ApplicationException("Nenhuma conta gráfica informada para atualização.");

        var contasSalvas = await _contaGraficaRepository.AtualizarContasAsync(contasAhSeremAtualizadas, cancellationToken);

        return contasSalvas;
    }

    public async Task<Result<CarteiraDto>> ObterRentabilidadeDaCertira(List<CustodiaFilhoteDto> custodias, CancellationToken cancellationToken)
    {
        var cotacoesFechamentoCesta = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cancellationToken);
        if (!cotacoesFechamentoCesta.IsSuccess)
            return new ApplicationException("Falha ao obter cotações do ativo na B3");

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
            return new ApplicationException("Falha ao obter cotações do ativo na B3");

        var historicoCompra = conta.HistoricoCompra!
            .DistinctBy(d => d.Data)
            .OrderBy(g => g.Data)
            .Select((t, index) => new HistoricoAporteDto(
                0,
                t.Data,
                t.ValorAporte,
                $"{index + 1}/3"
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