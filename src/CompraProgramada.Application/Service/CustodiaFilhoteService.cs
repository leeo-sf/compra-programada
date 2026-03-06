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
            CustodiaFilhotes = c.CustodiaFilhote!
                .Select(cf => new CustodiaFilhote(cf.Id, cf.ContaGraficaId, cf.Ticker, cf.PrecoMedio, cf.Quantidade)).ToList()
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
        decimal valorAtualCarteira = 0;
        decimal plTotal = 0;
        decimal saldoTotalCarteira = 0;

        foreach (var custodia in custodias)
        {
            var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;

            var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
            var saldoCarteira = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;
            var plAtivo = (fechamentoAtivo.PrecoFechamento - custodia.PrecoMedio) * custodia.Quantidade;

            plTotal += plAtivo;
            saldoTotalCarteira += saldoCarteira;
            valorTotalInvestido += valorInvestido;
            valorAtualCarteira += custodia.Quantidade + fechamentoAtivo.PrecoFechamento;
        }

        var rentabilidadePercentual = custodias.Sum(custodia =>
        {
            var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;

            var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
            var valorAtual = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;

            return (valorAtual / valorInvestido - 1) * 100;
        });

        var detalhesAtivos = custodias.Select(custodia =>
        {
            var fechamentoAtivo = cotacoesFechamentoCesta.Value.Itens.FirstOrDefault(x => x.Ticker == custodia.Ticker)!;
            var valorAtual = custodia.Quantidade * fechamentoAtivo.PrecoFechamento;
            var pl = (fechamentoAtivo.PrecoFechamento - custodia.PrecoMedio) * custodia.Quantidade;
            var plPercentual = (custodia.PrecoMedio / fechamentoAtivo.PrecoFechamento - 1) * 100;
            var composicaoCarteira = valorAtual / valorTotalInvestido * 100;

            return new DetalheAtivoCarteiraDto(
                custodia.Ticker,
                custodia.Quantidade,
                custodia.PrecoMedio,
                fechamentoAtivo.PrecoFechamento,
                valorAtual,
                pl,
                plPercentual,
                composicaoCarteira);
        }).ToList();

        return new CarteiraDto(
            new ResumoCarteiraDto(valorTotalInvestido, valorAtualCarteira, plTotal, rentabilidadePercentual), detalhesAtivos);
    }
}