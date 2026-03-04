using CompraProgramada.Application.Config;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Service;

public class MotorCompraService : IMotorCompraService
{
    private readonly ILogger<MotorCompraService> _logger;
    private readonly IHistoricoExecucaoMotorService _historicoExecucaoService;
    private readonly IClienteService _clienteService;
    private readonly ICotahistParserService _cotahistParser;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICalendarioMotorCompraService _calendarioMotorCompraService;
    private readonly ICustodiaMasterService _custodiaMasterService;

    public MotorCompraService(ILogger<MotorCompraService> logger,
        IHistoricoExecucaoMotorService historicoExecucaoService,
        IClienteService clienteService,
        ICotahistParserService cotahistParser,
        ICestaRecomendadaService cestaService,
        ICotacaoService cotacaoService,
        ICalendarioMotorCompraService calendarioMotorCompraService,
        ICustodiaMasterService custodiaMasterService)
    {
        _logger = logger;
        _historicoExecucaoService = historicoExecucaoService;
        _clienteService = clienteService;
        _cotahistParser = cotahistParser;
        _cestaService = cestaService;
        _cotacaoService = cotacaoService;
        _calendarioMotorCompraService = calendarioMotorCompraService;
        _custodiaMasterService = custodiaMasterService;
    }

    public async Task ExecutarCompraAsync(CancellationToken cancellationToken)
    {
        var deveExecutarCompraHoje = await _historicoExecucaoService.ExecutarCompraHojeAsync(cancellationToken);

        /*if (!deveExecutarCompraHoje)
        {
            var dataProximaExecucao = _calendarioMotorCompraService.ObterProximaDataCompra();
            _logger.LogInformation("MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.", dataProximaExecucao);
            return;
        }*/

        var clientesAtivos = await _clienteService.ObtemClientesAtivoAsync(cancellationToken);
        if (!clientesAtivos.IsSuccess || !clientesAtivos.Value!.Any())
            throw new ApplicationException("Erro ao obter clientes ativos");

        _logger.LogInformation("Obtido {QuantidadeClientes} clientes ativos para processamento.", clientesAtivos.Value!.Count);

        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess || cestaVigente.Value is null)
            throw new ApplicationException("Erro ao obter cesta vigente recomendada");

        _logger.LogInformation("Obtido cesta vigente CestaId {CestaId} para processamento.", cestaVigente.Value!.Id);

        var totalConsolidado = clientesAtivos.Value!.Sum(cliente => cliente.ValorMensal / 3);

        _logger.LogInformation("Total Consolidado a ser comprado: {TotalConsolidado}", totalConsolidado);

        var valorPorAtivo = ValorPorAtivoConsolidado(cestaVigente.Value!, totalConsolidado);

        _logger.LogInformation("Valor por ativo consolidade: {ValorPorAtivo}", valorPorAtivo);

        var cotacoesFechamento = await ObterCotacoesFechamentoB3ComBaseEmCestaAsync(cancellationToken);

        _logger.LogInformation("Cotações de fachamento da B3 com base na data pregão {DataPregao}. Cotações: {CotacoesFechamento}", cotacoesFechamento.DataPregao, cotacoesFechamento.Itens);

        var quantidadePorAtivo = await VerificarResiduosAsync(cotacoesFechamento.Itens, valorPorAtivo, cancellationToken);

        _logger.LogInformation("Quantidade de ativos a comprar após verificar resíduos: {AtivosAhComprar}", quantidadePorAtivo);

        var (grupoContasDistribuidas, residuos) = DistribuiGrupoEObtemResiduos(clientesAtivos.Value, quantidadePorAtivo, totalConsolidado, cancellationToken);

        _logger.LogInformation("Distribuição de grupo realizada e residuos definidos.");

        // salvar distribuição

        // 1. Calcula a parte do cliente (ex: 5 ações)
        // 2. Cria registro na tabela 'Distribuicao' (Log)
        // 3. Incrementa a 'CustodiaFilhote' do cliente (Saldo atualizado)
        // 4. Calcula IR Dedo-Duro e envia pro Kafka

        var result = await _custodiaMasterService.RegistrarResiduosAsync(residuos, cancellationToken);

        if (!result.IsSuccess)
            throw new ApplicationException(result.Exception!.Message);

        _logger.LogInformation("Residuos registrados na base. Serão considerados na próxima compra: {Residuos}", residuos);

        var dataExecucao = DateTime.Now;
        var dataReferencia = _calendarioMotorCompraService.ObterDataReferenciaExecucao(dataExecucao);
        await _historicoExecucaoService.SalvarExecucaoAsync(new ExecucaoMotorCompraDto { DataReferencia = dataReferencia, DataExecucao = dataExecucao }, cancellationToken);
    }

    public List<(string Ticker, decimal valorPorAtivo)> ValorPorAtivoConsolidado(CestaRecomendada cesta, decimal totalConsolidado)
        => cesta.ComposicaoCesta
            .Select(ativo => (ativo.Ticker, totalConsolidado * (ativo.Percentual / 100)))
            .ToList();

    public async Task<CotacaoDto> ObterCotacoesFechamentoB3ComBaseEmCestaAsync(CancellationToken cancellationToken)
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

    public async Task<List<AtivoDto>> VerificarResiduosAsync(List<ComposicaoCotacaoDto> cotacoesFechamento, List<(string Ticker, decimal ValorAhComprar)> valoresPorAtivoConsolidado, CancellationToken cancellationToken)
    {
        // verificar na base de dados se teve resíduos de compra

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

        // ordem de compra

        var quantidadeDeCompraPorAtivo = combinaoFechamentoECompra
            .Select(x => new AtivoDto { Ticker = x.Ticker, Quantidade = (int)Math.Truncate(x.ValorAhComprarPorAtivo / x.PrecoFechamento) })
            .ToList();

        return quantidadeDeCompraPorAtivo;
    }

    public List<LoteCompraDto> DefineLotePadraoOuFracionario(List<(string Ticker, int QuantidadeDeCompra)> ativosAhComprar)
        => ativosAhComprar.Select(ativo =>
        {
            if (ativo.QuantidadeDeCompra <= 0)
                throw new DivideByZeroException("Há ativos a ser comprados com a quantidade zerada.");

            return new LoteCompraDto
            {
                Ticker = ativo.Ticker,
                QuantidadeLotePadrao = ativo.QuantidadeDeCompra / 100 * 100,
                QuantidadeLoteFracionario = ativo.QuantidadeDeCompra % 100
            };
        }).ToList();

    public (List<ContaGrafica> grupoContasDistribuidas, List<AtivoDto> residuos) DistribuiGrupoEObtemResiduos(List<Cliente> clientes, List<AtivoDto> ativos, decimal totalConsolidado, CancellationToken cancellationToken)
    {
        var contasDistribuidas = new List<ContaGrafica>();
        var residuos = new List<AtivoDto>();
        foreach (var ativo in ativos)
        {
            int totalDistribuido = 0;

            foreach (var cliente in clientes)
            {
                var contaCliente = contasDistribuidas.FirstOrDefault(x => x.ClienteId == cliente.Id)!;
                var custodiaAtivo = contaCliente.CustodiaFilhotes.FirstOrDefault(cf => cf.Ticker == ativo.Ticker)!;

                var quantidadeNovasAcoes = (int)Math.Truncate(ativo.Quantidade * (cliente.ValorAporte / totalConsolidado));

                contaCliente.CustodiaFilhotes.Add(new (0, contaCliente.Id, ativo.Ticker, custodiaAtivo.Quantidade + quantidadeNovasAcoes));

                contasDistribuidas.Add(contaCliente);

                totalDistribuido += quantidadeNovasAcoes;
            }

            var residuo = Math.Abs(totalDistribuido - ativo.Quantidade);
            if (residuo > 0)
                residuos.Add((new AtivoDto { Ticker = ativo.Ticker, Quantidade = residuo }));
        }

        return (contasDistribuidas, residuos);
    }
}