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
    private readonly IOrdemCompraService _ordemCompraService;
    private readonly ICotahistParserService _cotahistParser;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly IFileService _fileService;

    public DistribuicaoService(ILogger<DistribuicaoService> logger,
        IDistribuicaoRepository distribuicaoRepository,
        ICustodiaMasterService custodiaMasterService,
        ICustodiaFilhoteService custodiaFilhoteService,
        IOrdemCompraService ordemCompraService,
        ICotahistParserService cotahistParser,
        ICotacaoService cotacaoService,
        ICestaRecomendadaService cestaService,
        IFileService fileService)
    {
        _logger = logger;
        _distribuicaoRepository = distribuicaoRepository;
        _custodiaMasterService = custodiaMasterService;
        _custodiaFilhoteService = custodiaFilhoteService;
        _ordemCompraService = ordemCompraService;
        _cotahistParser = cotahistParser;
        _cotacaoService = cotacaoService;
        _cestaService = cestaService;
        _fileService = fileService;
    }

    public async Task<Result<List<DistribuicaoDto>>> RealizarDistribuicaoAtivoPorCliente(List<ClienteDto> clientesAtivos, decimal totalConsolidado, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            throw new ApplicationException(cestaVigente.Exception!.Message);

        _logger.LogInformation("Obtido cesta vigente para processamento: {CestaRecomendada}", cestaVigente.Value);

        var residuos = await ObtemResiduosAsync(cancellationToken);

        var cotacoesFechamento = await ObterCotacoesFechamentoB3ComBaseEmCestaAsync(cancellationToken);

        var valoresPorAtivoConsolidado = _cestaService.ValorPorAtivoConsolidado(cestaVigente.Value!, totalConsolidado);
        if (!valoresPorAtivoConsolidado.IsSuccess || valoresPorAtivoConsolidado.Value is null)
            throw new ApplicationException("Erro ao obter valor por ativo consolidado");

        _logger.LogInformation("Valor por ativo consolidade: {ValorPorAtivo}", valoresPorAtivoConsolidado.Value);

        var combinacoesFechamentoECompra = cotacoesFechamento.Itens.Join(
            valoresPorAtivoConsolidado.Value!,
            cotacaoFechamento => cotacaoFechamento.Ticker,
            valorPorAtivoConsolidado => valorPorAtivoConsolidado.Ticker,
            (cotacaoFechamento, valorPorAtivoConsolidado) => new
            {
                Ticker = cotacaoFechamento.Ticker,
                PrecoFechamento = cotacaoFechamento.PrecoFechamento,
                ValorAhComprarPorAtivo = valorPorAtivoConsolidado.ValorDeCompraPorAtivo
            }).ToList();

        var ativosConsolidados = new List<(string Ticker, int Quantidade, decimal PrecoFechamento)>();
        var residuosAtualizados = new List<AtivoDto>();
        var ordensCompra = new List<OrdemCompraDto>();

        foreach (var combinacaoFechamento in combinacoesFechamentoECompra)
        {
            var temResiduoDoAtivo = residuos?.FirstOrDefault(r => r.Ticker == combinacaoFechamento.Ticker);

            var quantidadeDeCompraAtivo = (int)Math.Truncate(combinacaoFechamento.ValorAhComprarPorAtivo / combinacaoFechamento.PrecoFechamento);

            if (temResiduoDoAtivo is not null)
            {
                int quantidadeSobra = Math.Max(0, temResiduoDoAtivo.QuantidadeResiduos - quantidadeDeCompraAtivo);
                quantidadeDeCompraAtivo = Math.Max(0, quantidadeDeCompraAtivo - temResiduoDoAtivo.QuantidadeResiduos);

                residuosAtualizados.Add(new AtivoDto { Ticker = combinacaoFechamento.Ticker, Quantidade = quantidadeSobra });
            }

            ativosConsolidados.Add((
                Ticker: combinacaoFechamento.Ticker,
                Quantidade: quantidadeDeCompraAtivo,
                PrecoFechamento: combinacaoFechamento.PrecoFechamento
            ));

            ordensCompra.Add(new OrdemCompraDto
            {
                Id = 0,
                Ticker = combinacaoFechamento.Ticker,
                QuantidadeTotal = quantidadeDeCompraAtivo,
                PrecoExecucao = combinacaoFechamento.PrecoFechamento,
                QuantidadeLotePadrao = quantidadeDeCompraAtivo / 100 * 100 / 100,
            });
        }

        var ordensCompraEmitidas = await _ordemCompraService.EmitirOrdensDeCompraAsync(ordensCompra, cancellationToken);
        if (!ordensCompraEmitidas.IsSuccess || ordensCompraEmitidas.Value is null)
            throw new ApplicationException(ordensCompraEmitidas.Exception!.Message);

        _logger.LogInformation("Ordens de Compra emitidas: {OrdensCompra}", ordensCompraEmitidas.Value);

        await _custodiaMasterService.AjustarResiduosAsync(residuosAtualizados, cancellationToken);

        _logger.LogInformation("Resíduos atualizados na base: {Residuos}", residuosAtualizados);

        var distribuicao = await DistribuicaoParaContasGrafica(clientesAtivos, ativosConsolidados, totalConsolidado, cancellationToken);

        _logger.LogInformation("Distribuições para os clientes realizadas: {OrdensCompra}", ordensCompraEmitidas.Value);

        await SalvarRegistroDistribuicoes(distribuicao, ordensCompraEmitidas.Value, cancellationToken);

        _logger.LogInformation("Registrando distribuições na base de dados");

        return distribuicao;
    }

    public async Task<List<DistribuicaoDto>> DistribuicaoParaContasGrafica(List<ClienteDto> clientes, List<(string Ticker, int Quantidade, decimal PrecoFechamento)> ativosConsolidado, decimal totalConsolidado, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de distribuição dos ativos para os clientes. Cesta: {Ativos}", ativosConsolidado);
        var distribuicao = new List<DistribuicaoDto>();
        var contasClientesAtualizadas = new List<ContaGraficaDto>();

        foreach (var cliente in clientes)
        {
            foreach (var ativo in ativosConsolidado)
            {
                var contaCliente = cliente.ContaGrafica!;
                var custodiaAtualCliente = contaCliente.CustodiaFilhote?.FirstOrDefault(
                    x => x.Ticker == ativo.Ticker && x.ContaGraficaId == contaCliente.Id);

                var quantidadeNovasAcoes = (int)Math.Truncate(ativo.Quantidade * (cliente.ValorAporte / totalConsolidado));

                var custodiaAhSerAtualizada = contasClientesAtualizadas.FirstOrDefault(x => x.Id == contaCliente.Id);

                var custodiaClienteAtualizada = new CustodiaFilhoteDto
                {
                    Id = custodiaAtualCliente!.Id,
                    ContaGraficaId = custodiaAtualCliente.ContaGraficaId,
                    Ticker = custodiaAtualCliente.Ticker!,
                    Quantidade = custodiaAtualCliente.Quantidade + quantidadeNovasAcoes
                };

                if (custodiaAhSerAtualizada is null)
                    contasClientesAtualizadas.Add(new ContaGraficaDto
                    {
                        Id = contaCliente.Id,
                        NumeroConta = contaCliente.NumeroConta,
                        ClienteId = contaCliente.ClienteId,
                        Tipo = contaCliente.Tipo,
                        DataCriacao = contaCliente.DataCriacao,
                        CustodiaFilhote = new List<CustodiaFilhoteDto>() { custodiaClienteAtualizada }
                    });
                else
                    contasClientesAtualizadas.Find(x => x.Id == contaCliente.Id)?.CustodiaFilhote?.Add(custodiaClienteAtualizada);

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
        _logger.LogInformation("Salvando custodias filhotes na base de dados.");

        return distribuicao;
    }

    public async Task<List<CustodiaMasterDto>?> ObtemResiduosAsync(CancellationToken cancellationToken)
    {
        var residuos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);

        if (residuos.Value is null)
            return null;

        return residuos.Value;
    }

    public async Task<CotacaoDto> ObterCotacoesFechamentoB3ComBaseEmCestaAsync(CancellationToken cancellationToken)
    {
        var caminhoArquivoUltimoPregao = _fileService.ObterCaminhoCompletoArquivoCotacoes();

        var cotacoesB3 = _cotahistParser.ParseArquivo(caminhoArquivoUltimoPregao);

        var cestaVigente = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (!cestaVigente.IsSuccess)
            throw new ApplicationException(cestaVigente.Exception.Message);

        var cestaVigenteTickers = new HashSet<string>(cestaVigente.Value.Itens.Select(x => x.Ticker), StringComparer.OrdinalIgnoreCase);

        var cotacoesCesta = cotacoesB3.Where(cotacao => cestaVigenteTickers.Contains(cotacao.Ticker));

        if (!cotacoesCesta.Any())
            throw new ApplicationException("Não foi possível obter a cesta recomendada nas cotações da B3.");

        var result = new CotacaoDto { DataPregao = cotacoesCesta.FirstOrDefault()!.DataPregao, Itens = cotacoesCesta.Select(x => new ComposicaoCotacaoDto(x.Ticker, x.PrecoFechamento)).ToList() };

        _logger.LogInformation("Cotações de fachamento B3 da cesta Top Five com base na data pregão {DataPregao}. Cotações: {CotacoesFechamento}", result.DataPregao, result.Itens);

        await _cotacaoService.SalvarCotacaoAsync(
            result,
            cancellationToken);

        return result;
    }

    public async Task SalvarRegistroDistribuicoes(List<DistribuicaoDto> ditribuicoes, List<OrdemCompraDto> ordensCompraAtivos, CancellationToken cancellationToken)
    {
        var distribuicoesAhSeremSalvas = ditribuicoes.Select(d => new Distribuicao(
            0, ordensCompraAtivos.First(x => x.Ticker == d.Ticker).Id, d.ContaGraficaId, d.Ticker, d.QuantidadeAlocada, d.ValorOperacao)).ToList();

        await _distribuicaoRepository.SalvarDistribuicoesAsync(distribuicoesAhSeremSalvas, cancellationToken);
    }
}