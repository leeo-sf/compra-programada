using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class DistribuicaoService : IDistribuicaoService
{
    private readonly ILogger<DistribuicaoService> _logger;
    private readonly IDistribuicaoRepository _distribuicaoRepository;
    private readonly ICustodiaMasterService _custodiaMasterService;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly IPrecoMedioService _precoMedioService;
    private readonly IContaGraficaService _contaGraficaService;

    public DistribuicaoService(ILogger<DistribuicaoService> logger,
        IDistribuicaoRepository distribuicaoRepository,
        ICustodiaMasterService custodiaMasterService,
        ICotahistParserService cotahistParser,
        ICotacaoService cotacaoService,
        ICestaRecomendadaService cestaService,
        IPrecoMedioService precoMedioService,
        IContaGraficaService contaGraficaService)
    {
        _logger = logger;
        _distribuicaoRepository = distribuicaoRepository;
        _custodiaMasterService = custodiaMasterService;
        _cotacaoService = cotacaoService;
        _cestaService = cestaService;
        _precoMedioService = precoMedioService;
        _contaGraficaService = contaGraficaService;
    }

    public async Task<(List<DistribuicaoDto>, List<AtivoAhCompraDto>)> RealizarDistribuicoesAsync(List<ClienteDto> clientesAtivos, DateTime dataExecucao, List<OrdemCompraDto> ordensCompra, CancellationToken cancellationToken)
    {
        var totalConsolidado = clientesAtivos.Sum(cliente => cliente.ValorMensal / 3);

        var combinacoesFechamentoECompra = await _cotacaoService.ObterCombinacoesFechamentoECompraAtivoAsync(totalConsolidado, cancellationToken);
        if (!combinacoesFechamentoECompra.IsSuccess)
            throw combinacoesFechamentoECompra.Exception;

        var residuosNaoDistribuidos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosNaoDistribuidos.IsSuccess)
            throw residuosNaoDistribuidos.Exception;

        var grupoAtivosAhDistribuir = new List<AtivoAhCompraDto>();
        var residuosAtualizados = new List<ResiduoCustodiaMasterDto>();

        foreach (var fechamento in combinacoesFechamentoECompra.Value)
        {
            var custodia = residuosNaoDistribuidos.Value.FirstOrDefault(x => x.Ticker == fechamento.Ticker);
            var residuosAtuais = custodia?.QuantidadeResiduos ?? 0;
            var qtdNecessariaParaDistribuicao = (int)Math.Truncate(fechamento.ValorAhCompra / fechamento.PrecoFechamento);

            var quantidadeDeCompraAtivo = _custodiaMasterService.SubtrairResiduosParaCompra(custodia, qtdNecessariaParaDistribuicao);
            var qtdSobraResiduos = residuosAtuais - Math.Abs(quantidadeDeCompraAtivo - qtdNecessariaParaDistribuicao);

            var grupo = new ResiduoCustodiaMasterDto(fechamento.Ticker, qtdSobraResiduos);

            grupoAtivosAhDistribuir.Add(new AtivoAhCompraDto(fechamento.Ticker, qtdNecessariaParaDistribuicao, fechamento.PrecoFechamento));
            residuosAtualizados.Add(grupo);
        }

        var residuosAtualizadosResult = await _custodiaMasterService.AtualizarResiduosAsync(residuosAtualizados, cancellationToken);
        if (!residuosAtualizadosResult.IsSuccess)
            throw residuosAtualizadosResult.Exception;

        var distribuicoesRealizadasResult = await DistribuirCustodiasAsync(clientesAtivos, grupoAtivosAhDistribuir, totalConsolidado, dataExecucao, ordensCompra, cancellationToken);
        if (!distribuicoesRealizadasResult.IsSuccess)
            throw distribuicoesRealizadasResult.Exception;

        _logger.LogInformation("Distribuição para as custodias realizada.");

        return (distribuicoesRealizadasResult.Value, grupoAtivosAhDistribuir);
    }

    public async Task<Result<List<DistribuicaoDto>>> DistribuirCustodiasAsync(List<ClienteDto> clientes, List<AtivoAhCompraDto> grupoAtivoCompra, decimal totalConsolidado, DateTime dataExeucao, List<OrdemCompraDto> ordensCompra, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de distribuição dos ativos para os clientes. Grupo: {Ativos}", grupoAtivoCompra);
        var distribuicao = new List<DistribuicaoDto>();
        var ativos = new List<AtivoDto>();
        var contasClientesAtualizadas = new List<ContaGraficaDto>();

        foreach (var cliente in clientes.OrderByDescending(x => x.ClienteId))
        {
            foreach (var ativo in grupoAtivoCompra)
            {
                var contaCliente = cliente.ContaGrafica!;
                var custodiaAtualCliente = contaCliente.CustodiaFilhotes?.FirstOrDefault(
                    x => x.Ticker == ativo.Ticker && x.ContaGraficaId == contaCliente.Id);

                var quantidadeNovasAcoes = (int)Math.Truncate(ativo.Quantidade * (cliente.ValorAporte / totalConsolidado));

                var precoMedio = _precoMedioService.CalcularPrecoMedio(new PrecoMedioDto { PrecoMedioAnterior = custodiaAtualCliente!.PrecoMedio, PrecoFechamentoAtivo = ativo.PrecoFechamento, QuantidadeAtivosAnterior = custodiaAtualCliente.Quantidade, QuantidadeNovosAtivos = quantidadeNovasAcoes });

                var custodiaAhSerAtualizada = contasClientesAtualizadas.FirstOrDefault(x => x.Id == contaCliente.Id);

                var custodiaClienteAtualizada = new CustodiaFilhoteDto(
                    custodiaAtualCliente!.Id,
                    custodiaAtualCliente.ContaGraficaId,
                    custodiaAtualCliente.Ticker!,
                    precoMedio.Value,
                    custodiaAtualCliente.Quantidade + quantidadeNovasAcoes
                );

                var historicoCompra = new HistoricoCompraDto(
                    contaCliente.Id,
                    ativo.Ticker,
                    quantidadeNovasAcoes,
                    cliente.ValorAporte,
                    ativo.PrecoFechamento,
                    precoMedio.Value,
                    DateOnly.FromDateTime(dataExeucao));

                if (custodiaAhSerAtualizada is null)
                    contasClientesAtualizadas.Add(new ContaGraficaDto(
                        contaCliente.Id,
                        contaCliente.NumeroConta,
                        DateTime.Now,
                        contaCliente.ClienteId,
                        contaCliente.Tipo,
                        new List<HistoricoCompraDto>() { historicoCompra },
                        new List<CustodiaFilhoteDto>() { custodiaClienteAtualizada }
                    ));
                else
                {
                    contasClientesAtualizadas.Find(x => x.Id == contaCliente.Id)?.CustodiaFilhotes?.Add(custodiaClienteAtualizada);
                    contasClientesAtualizadas.Find(x => x.Id == contaCliente.Id)?.HistoricoCompra?.Add(historicoCompra);
                }

                distribuicao.Add(new DistribuicaoDto(
                    0,
                    cliente.Cpf,
                    0,
                    custodiaAtualCliente.ContaGraficaId,
                    ativo.Ticker,
                    quantidadeNovasAcoes,
                    quantidadeNovasAcoes * ativo.PrecoFechamento,
                    contasClientesAtualizadas.FirstOrDefault(x => x.Id == contaCliente.Id)!,
                    dataExeucao,
                    cliente.ClienteId,
                    cliente.Nome,
                    cliente.ValorAporte,
                    new List<AtivoDto> { new AtivoDto(ativo.Ticker, quantidadeNovasAcoes) }
                ));
            }
        }

        await _contaGraficaService.AtualizarCustodiasContasAsync(contasClientesAtualizadas, cancellationToken);
        _logger.LogInformation("Atualizando custodias das contas ativas na base de dados.");

        var historicosCompra = contasClientesAtualizadas.SelectMany(x => x.HistoricoCompra!).ToList();
        await _contaGraficaService.RegistrarComprasAsync(historicosCompra, cancellationToken);
        _logger.LogInformation("Registrado o histórico de compras dos clientes na base.");

        await SalvarDistribuicoesAsync(distribuicao, ordensCompra, cancellationToken);

        return distribuicao;
    }

    public async Task<Result> SalvarDistribuicoesAsync(List<DistribuicaoDto> ditribuicoes, List<OrdemCompraDto> ordensCompraAtivos, CancellationToken cancellationToken)
    {
        var distribuicoesAhSeremSalvas = ditribuicoes
            .Select(d => Distribuicao.CriarDistribuicao(
                ordensCompraAtivos.First(x => x.Ticker == d.Ticker).Id, d.ContaGraficaId, d.Ticker, d.QuantidadeAlocada, d.ValorOperacao)).ToList();

        await _distribuicaoRepository.SalvarDistribuicoesAsync(distribuicoesAhSeremSalvas, cancellationToken);

        _logger.LogInformation("Registrado distribuições de custodias na base de dados");

        return Result.Success();
    }
}