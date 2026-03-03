using CompraProgramada.Application.Config;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Service;

public class MotorCompraService : IMotorCompraService
{
    private readonly ILogger<MotorCompraService> _logger;
    private readonly AppConfig _appConfig;
    private readonly IHistoricoExecucaoMotorService _historicoExecucaoService;
    private readonly IClienteService _clienteService;
    private readonly ICotahistParser _cotahistParser;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly ICotacaoService _cotacaoService;

    public MotorCompraService(ILogger<MotorCompraService> logger,
        AppConfig appConfig,
        IHistoricoExecucaoMotorService historicoExecucaoService,
        IClienteService clienteService,
        ICotahistParser cotahistParser,
        ICestaRecomendadaService cestaService,
        ICotacaoService cotacaoService)
    {
        _logger = logger;
        _appConfig = appConfig;
        _historicoExecucaoService = historicoExecucaoService;
        _clienteService = clienteService;
        _cotahistParser = cotahistParser;
        _cestaService = cestaService;
        _cotacaoService = cotacaoService;
    }

    public async Task ExecutarCompraAsync(CancellationToken cancellationToken)
    {
        var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);

        if (!deveExecutarCompraHoje)
        {
            _logger.LogInformation("MotorCompra não será executado hoje. Encerrando processo.");
            return;
        }

        var clientesAtivos = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);

        if (!clientesAtivos.IsSuccess || !clientesAtivos.Value!.Any())
            throw new ApplicationException("Erro ao obter clientes ativos");

        _logger.LogInformation("Obtido {QuantidadeClientes} clientes ativos para processamento.", clientesAtivos.Value!.Count);

        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (!cestaVigente.IsSuccess || cestaVigente.Value is null)
            throw new ApplicationException("Erro ao obter cesta vigente recomendada");

        _logger.LogInformation("Obtido cesta vigente CestaId {CestaId} para processamento.", cestaVigente.Value!.Id);

        var totalConsolidadoCompra = clientesAtivos.Value!.Sum(cliente => cliente.ValorMensal / 3);

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", totalConsolidadoCompra);

        var valoresPorAtivoConsolidado = ValorPorAtivoConsolidado(cestaVigente.Value!, totalConsolidadoCompra);

        _logger.LogInformation("Valor por ativo consolidade: {ValorPorAtivo}", valoresPorAtivoConsolidado);

        var cotacoesFechamento = await ObterCotacoesFechamentoB3ComBaseEmCestaAsync(cancellationToken);

        _logger.LogInformation("Cotações de fachamento da B3 com base na data pregão {DataPregao}. Cotações: {CotacoesFechamento}", cotacoesFechamento.DataPregao, cotacoesFechamento.Itens);

        var quantidadeAhComprarPorAtivo = VerificarResiduosAsync(cotacoesFechamento.Itens, valoresPorAtivoConsolidado, cancellationToken);
    }

    private List<(string Ticker, decimal ValorAhComprar)> ValorPorAtivoConsolidado(CestaRecomendada cesta, decimal totalConsolidadoCompra)
        => cesta.ComposicaoCesta
            .Select(ativo => (ativo.Ticker, totalConsolidadoCompra * ativo.Percentual))
            .ToList();

    private async Task<CotacaoDto> ObterCotacoesFechamentoB3ComBaseEmCestaAsync(CancellationToken cancellationToken)
    {
        string pastaCotacoes = Path.Combine(AppContext.BaseDirectory, "cotacoes");

        if (!Directory.Exists(pastaCotacoes))
            throw new DirectoryNotFoundException($"A pasta {pastaCotacoes} não foi encontrada.");

        var arquivoUltimoPregao = Directory.GetFiles(pastaCotacoes, "COTAHIST_D*")
            .Select(caminho => new FileInfo(caminho))
            .OrderByDescending(f => f.Name)
            .FirstOrDefault();

        if (!File.Exists(arquivoUltimoPregao!.FullName))
            throw new FileNotFoundException($"Nenhum arquivo com a data pregão mais recente foi encontrado.");

        var cotacoesB3 = _cotahistParser.ParseArquivo(arquivoUltimoPregao.FullName);
        
        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);
        var cestaVigenteTickers = new HashSet<string>(cestaVigente.Value!.ComposicaoCesta.Select(x => x.Ticker), StringComparer.OrdinalIgnoreCase);

        var cotacoesCesta = cotacoesB3.Where(cotacao => cestaVigenteTickers.Contains(cotacao.Ticker));

        var result = new CotacaoDto { DataPregao = cotacoesCesta.First().DataPregao, Itens = cotacoesCesta.Select(x => new ComposicaoCotacaoDto(x.Ticker, x.PrecoFechamento)).ToList() };

        await _cotacaoService.SalvarCotacaoAsync(
            result,
            cancellationToken);

        return result;
    }

    private async Task<List<(string Ticker, int QuantidadeDeCompra)>> VerificarResiduosAsync(List<ComposicaoCotacaoDto> cotacoesFechamento, List<(string Ticker, decimal ValorAhComprar)> valoresPorAtivoConsolidado, CancellationToken cancellationToken)
    {
        var combinaoFechamentoECompra = cotacoesFechamento.Join(
            valoresPorAtivoConsolidado,
            cotacaoFechamento => cotacaoFechamento.Ticker,
            valorPorAtivoConsolidado => valorPorAtivoConsolidado.Ticker,
            (cotacaoFechamento, valorPorAtivoConsolidado) => new
            {
                Ticker = cotacaoFechamento.Ticker,
                PrecoFechamento = cotacaoFechamento.PrecoFechamento,
                ValorAhComprarPorAtivo = valorPorAtivoConsolidado.ValorAhComprar
            }).ToList();

        // verificar na base de dados se teve resíduos de compra

        var quantidadeDeCompraPorAtivo = combinaoFechamentoECompra
            .Select(x => (x.Ticker, (int)Math.Truncate(x.ValorAhComprarPorAtivo / x.PrecoFechamento)))
            .ToList();

        return quantidadeDeCompraPorAtivo;
    }

    private List<(string Ticker, int LotePadrao, int LoteFracionario)> DefinirLotePadraoOuFracionario(List<(string Ticker, int QuantidadeDeCompra)> ativosAhComprar)
    {
        throw new NotImplementedException();
    }
}