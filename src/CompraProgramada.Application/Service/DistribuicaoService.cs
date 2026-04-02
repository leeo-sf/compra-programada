using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Domain.Entity;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class DistribuicaoService : IDistribuicaoService
{
    private readonly ILogger<DistribuicaoService> _logger;
    private readonly ICustodiaMasterService _custodiaMasterService;
    private readonly IContaGraficaService _contaGraficaService;

    public DistribuicaoService(ILogger<DistribuicaoService> logger,
        ICustodiaMasterService custodiaMasterService,
        IContaGraficaService contaGraficaService)
    {
        _logger = logger;
        _custodiaMasterService = custodiaMasterService;
        _contaGraficaService = contaGraficaService;
    }

    public async Task<Result<List<Distribuicao>>> DistribuirParaCustodiasAsync(List<Cliente> clientesAtivos, List<OrdemCompra> ordensCompra, DateTime dataExecucao, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de distribuição dos ativos para os clientes...");

        var residuosAtuaisResult = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosAtuaisResult.IsSuccess)
            return residuosAtuaisResult.Exception;

        _logger.LogInformation("Resíduos não distribuídos obtidos para reaproveitamento na distribuição: {Residuos}", residuosAtuaisResult.Value);

        var contasAhSeremAtuailzadas = CalcularDistribuicao(clientesAtivos, ordensCompra, residuosAtuaisResult.Value, dataExecucao);

        var contasAtualizadasResult = await _contaGraficaService.AtualizarContasAsync(contasAhSeremAtuailzadas, cancellationToken);
        if (!contasAtualizadasResult.IsSuccess)
            return contasAtualizadasResult.Exception;

        _logger.LogInformation("Atualização realizada das nas contas que tiveram a distribuição na base de dados.");

        return contasAtualizadasResult.Value.SelectMany(x => x.Distribuicoes).ToList();
    }

    public List<ContaGrafica> CalcularDistribuicao(List<Cliente> clientesAtivos, List<OrdemCompra> ordensCompra, List<CustodiaMaster> residuosAtuais, DateTime dataExecucao)
    {
        var valorTotalAportes = clientesAtivos.Sum(cliente => cliente.ValorMensal / 3);

        foreach (var ativo in ordensCompra)
        {
            var custodiaMasterAtivo = residuosAtuais.FirstOrDefault(r => r.Ticker == ativo.Ticker);
            var qtdTotalAtivoParaDistribuicao = ativo.QuantidadeTotal + (custodiaMasterAtivo?.QuantidadeResiduo ?? 0);

            foreach (var cliente in clientesAtivos.OrderByDescending(x => x.Id))
            {
                var contaCliente = cliente.ContaGrafica;
                var custodiaCliente = contaCliente.CustodiaFilhotes.FirstOrDefault(x => x.Ticker == ativo.Ticker && x.ContaGraficaId == contaCliente.Id)
                    ?? CustodiaFilhote.GerarCustodia(ativo.Ticker);

                var novaQuantidadeDeAtivos = (int)Math.Truncate(qtdTotalAtivoParaDistribuicao * (cliente.ValorAporte / valorTotalAportes));

                var valorPrecoMedio = custodiaCliente?.CalcularPrecoMedio(ativo.PrecoUnitario, novaQuantidadeDeAtivos) ?? 0;

                custodiaCliente!.AdicionarNovaQuantidade(novaQuantidadeDeAtivos);

                contaCliente.AdicionarCompra(HistoricoCompra.RegistrarHistorico(
                    contaCliente.Id,
                    ativo.Ticker,
                    novaQuantidadeDeAtivos,
                    ativo.PrecoUnitario,
                    valorPrecoMedio,
                    cliente.ValorAporte,
                    DateOnly.FromDateTime(dataExecucao)));

                contaCliente.AdicionarDistribuicao(
                    Distribuicao.CriarDistribuicao(novaQuantidadeDeAtivos, contaCliente, ativo));
            }
        }
        _logger.LogInformation("Distribuição realizada entre as custodias dos clientes ativos.");

        return clientesAtivos.Select(x => x.ContaGrafica).ToList();
    }
}