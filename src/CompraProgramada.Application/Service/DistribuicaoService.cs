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
    private readonly ICustodiaFilhoteService _custodiaFilhoteService;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly IPrecoMedioService _precoMedioService;
    private readonly IContaGraficaService _contaGraficaService;

    public DistribuicaoService(ILogger<DistribuicaoService> logger,
        IDistribuicaoRepository distribuicaoRepository,
        ICustodiaMasterService custodiaMasterService,
        ICustodiaFilhoteService custodiaFilhoteService,
        ICotahistParserService cotahistParser,
        ICotacaoService cotacaoService,
        ICestaRecomendadaService cestaService,
        IPrecoMedioService precoMedioService,
        IContaGraficaService contaGraficaService)
    {
        _logger = logger;
        _distribuicaoRepository = distribuicaoRepository;
        _custodiaMasterService = custodiaMasterService;
        _custodiaFilhoteService = custodiaFilhoteService;
        _cotacaoService = cotacaoService;
        _cestaService = cestaService;
        _precoMedioService = precoMedioService;
        _contaGraficaService = contaGraficaService;
    }

    public async Task<(List<GrupoAtivoCompraDto>, List<OrdemCompraDto>)> RealizaDistribuicaoGrupoAtivo(List<ClienteDto> clientesAtivos, decimal totalConsolidado, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            throw new ApplicationException(cestaVigente.Exception!.Message);

        _logger.LogInformation("Obtido cesta vigente para processamento: {CestaRecomendada}", cestaVigente.Value);

        var valoresPorAtivoConsolidado = _cestaService.ValorPorAtivoConsolidado(cestaVigente.Value!, totalConsolidado);
        if (!valoresPorAtivoConsolidado.IsSuccess || valoresPorAtivoConsolidado.Value is null)
            throw new ApplicationException("Erro ao obter valor por ativo consolidado");

        _logger.LogInformation("Valor por ativo consolidade: {ValorPorAtivo}", valoresPorAtivoConsolidado.Value);

        var cotacoesFechamento = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cancellationToken);
        if (!cotacoesFechamento.IsSuccess)
            throw cotacoesFechamento.Exception;

        var combinacoesFechamentoECompra = cotacoesFechamento.Value.Itens.Join(
            valoresPorAtivoConsolidado.Value!,
            cotacaoFechamento => cotacaoFechamento.Ticker,
            valorPorAtivoConsolidado => valorPorAtivoConsolidado.Ticker,
            (cotacaoFechamento, valorPorAtivoConsolidado) => new
            {
                Ticker = cotacaoFechamento.Ticker,
                PrecoFechamento = cotacaoFechamento.PrecoFechamento,
                ValorAhComprarPorAtivo = valorPorAtivoConsolidado.ValorDeCompraPorAtivo
            }).ToList();

        var residuosNaoDistribuidos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosNaoDistribuidos.IsSuccess)
            throw residuosNaoDistribuidos.Exception;

        var grupoAtivosAhDistribuir = new List<GrupoAtivoCompraDto>();
        var residuosAtualizados = new List<GrupoAtivoCompraDto>();
        var ordensCompra = new List<OrdemCompraDto>();

        foreach (var fechamento in combinacoesFechamentoECompra)
        {
            var custodia = residuosNaoDistribuidos.Value.FirstOrDefault(x => x.Ticker == fechamento.Ticker)!;
            var residuosAtuais = custodia.QuantidadeResiduos;
            var qtdNecessariaParaDistribuicao = (int)Math.Truncate(fechamento.ValorAhComprarPorAtivo / fechamento.PrecoFechamento);

            var quantidadeDeCompraAtivo = _custodiaMasterService.SubtrairResiduosParaCompra(custodia!, qtdNecessariaParaDistribuicao);
            var qtdSobraResiduos = residuosAtuais - Math.Abs(quantidadeDeCompraAtivo - qtdNecessariaParaDistribuicao);

            var grupo = new GrupoAtivoCompraDto(fechamento.Ticker, qtdSobraResiduos, fechamento.PrecoFechamento);

            grupoAtivosAhDistribuir.Add(grupo with { Quantidade = qtdNecessariaParaDistribuicao });
            residuosAtualizados.Add(grupo);

            ordensCompra.Add(new OrdemCompraDto
            {
                Id = 0,
                PrecoExecucao = fechamento.PrecoFechamento,
                QuantidadeCompra = quantidadeDeCompraAtivo,
                Ticker = fechamento.Ticker
            });
        }

        var residuosAtualizadosResult = await _custodiaMasterService.AtualizarResiduosAsync(residuosAtualizados, cancellationToken);
        if (!residuosAtualizadosResult.IsSuccess)
            throw residuosAtualizadosResult.Exception;

        return (grupoAtivosAhDistribuir, ordensCompra);
    }

    public async Task<Result<List<DistribuicaoDto>>> DistribuirCustodiasPorAtivo(List<ClienteDto> clientes, List<GrupoAtivoCompraDto> grupoAtivoCompra, decimal totalConsolidado, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de distribuição dos ativos para os clientes. Grupo: {Ativos}", grupoAtivoCompra);
        var distribuicao = new List<DistribuicaoDto>();
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
                    0,
                    cliente.ValorAporte,
                    DateOnly.FromDateTime(DateTime.Now),
                    contaCliente.Id);

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

                distribuicao.Add(new DistribuicaoDto
                {
                    Id = 0,
                    Cpf = cliente.Cpf,
                    OrdemCompraId = 0,
                    ContaGraficaId = custodiaAtualCliente.ContaGraficaId,
                    Ticker = ativo.Ticker,
                    QuantidadeAlocada = quantidadeNovasAcoes,
                    ValorOperacao = quantidadeNovasAcoes * ativo.PrecoFechamento,
                    ContaGrafica = contasClientesAtualizadas.FirstOrDefault(x => x.Id == contaCliente.Id)!,
                    Data = DateTime.Now
                });
            }
        }

        await _custodiaFilhoteService.AtualizarCustodiaFilhoteContasAsync(contasClientesAtualizadas, cancellationToken);
        _logger.LogInformation("Atualizando custodias das contas ativas na base de dados.");

        var historicosCompra = contasClientesAtualizadas.SelectMany(x => x.HistoricoCompra!).ToList();
        await _contaGraficaService.RegistrarComprasAsync(historicosCompra, cancellationToken);
        _logger.LogInformation("Registrado o histórico de compras dos clientes na base.");

        return distribuicao;
    }

    public async Task<Result> SalvarRegistroDistribuicoes(List<DistribuicaoDto> ditribuicoes, List<OrdemCompraDto> ordensCompraAtivos, CancellationToken cancellationToken)
    {
        var distribuicoesAhSeremSalvas = ditribuicoes.Select(d => new Distribuicao(
            0, ordensCompraAtivos.First(x => x.Ticker == d.Ticker).Id, d.ContaGraficaId, d.Ticker, d.QuantidadeAlocada, d.ValorOperacao)).ToList();

        await _distribuicaoRepository.SalvarDistribuicoesAsync(distribuicoesAhSeremSalvas, cancellationToken);

        _logger.LogInformation("Registrado distribuições de custodias na base de dados");

        return Result.Success();
    }
}