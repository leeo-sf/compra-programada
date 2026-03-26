using CompraProgramada.Application.Interface;
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

    public async Task<Result<List<Distribuicao>>> DistribuirParaCustodiasAsync(List<Cliente> clientesAtivos, List<OrdemCompra> ordensCompra, DateTime dataExeucao, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de distribuição dos ativos para os clientes...");

        var valorTotalAportes = clientesAtivos.Sum(cliente => cliente.ValorMensal / 3);

        var residuosNaoDistribuidos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosNaoDistribuidos.IsSuccess)
            throw residuosNaoDistribuidos.Exception;

        _logger.LogInformation("Resíduos não distribuídos obtidos para reaproveitamento na distribuição: {Residuos}", residuosNaoDistribuidos.Value);

        foreach (var ativo in ordensCompra)
        {
            var custodiaMasterAtivo = residuosNaoDistribuidos.Value.FirstOrDefault(r => r.Ticker == ativo.Ticker);
            var qtdTotalAtivoParaDistribuicao = ativo.QuantidadeTotal + custodiaMasterAtivo?.QuantidadeResiduo ?? 0;

            foreach (var cliente in clientesAtivos.OrderByDescending(x => x.Id))
            {
                var contaCliente = cliente.ContaGrafica;
                var custodiaCliente = contaCliente.CustodiaFilhotes.FirstOrDefault(x => x.Ticker == ativo.Ticker && x.ContaGraficaId == contaCliente.Id);

                var novaQuantidadeDeAtivos = (int)Math.Truncate(qtdTotalAtivoParaDistribuicao * (cliente.ValorAporte / valorTotalAportes));

                var valorPrecoMedio = custodiaCliente?.CalcularPrecoMedio(ativo.PrecoUnitario, novaQuantidadeDeAtivos) ?? 0;

                custodiaCliente!.AdicionarNovaQuantidade(novaQuantidadeDeAtivos);

                var distribuicao = Distribuicao.CriarDistribuicao(novaQuantidadeDeAtivos, contaCliente, ativo);

                contaCliente.AdicionarCompra(HistoricoCompra.RegistrarHistorico(
                    contaCliente.Id,
                    ativo.Ticker,
                    novaQuantidadeDeAtivos,
                    ativo.PrecoUnitario,
                    valorPrecoMedio,
                    cliente.ValorAporte,
                    DateOnly.FromDateTime(dataExeucao)));

                contaCliente.AdicionarDistribuicao(distribuicao);
            }
        }
        _logger.LogInformation("Distribuição realizada entre as custodias dos clientes ativos.");

        var contasAhSeremAtuailzadas = clientesAtivos.Select(x => x.ContaGrafica).ToList();

        var contasAtualizadasResult = await _contaGraficaService.AtualizarContasAsync(contasAhSeremAtuailzadas, cancellationToken);
        if (!contasAtualizadasResult.IsSuccess)
            throw contasAtualizadasResult.Exception;

        _logger.LogInformation("Atualização realizada das nas contas que tiveram a distribuição na base de dados.");

        return contasAtualizadasResult.Value.SelectMany(x => x.Distribuicoes).ToList();
    }
}