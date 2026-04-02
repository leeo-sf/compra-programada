using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class OrdemCompraService : IOrdemCompraService
{
    private readonly ILogger<OrdemCompraService> _logger;
    private readonly IOrdemCompraRepository _ordemCompraRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICustodiaMasterService _custodiaMasterService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;

    public OrdemCompraService(ILogger<OrdemCompraService> logger,
        IOrdemCompraRepository ordemCompraRepository,
        ICotacaoService cotacaoService,
        ICustodiaMasterService custodiaMasterService,
        ICestaRecomendadaService cestaRecomendadaService)
    {
        _logger = logger;
        _ordemCompraRepository = ordemCompraRepository;
        _cotacaoService = cotacaoService;
        _custodiaMasterService = custodiaMasterService;
        _cestaRecomendadaService = cestaRecomendadaService;
    }

    public async Task<Result<List<OrdemCompra>>> EmitirOrdensDeCompraAsync(decimal valorTotalConsolidado, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando emissão de ordens de compra...");

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return new ApplicationException(cestaVigente.Exception.Message);

        var fechamentosResult = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente.Value, cancellationToken);
        if (!fechamentosResult.IsSuccess)
            return fechamentosResult.Exception;

        var fechamentos = fechamentosResult.Value;

        _logger.LogInformation("Fechamento dos ativos correspondentes a cesta atual obtidos: {Fechamentos}", fechamentos);

        var residuosNaoDistribuidos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosNaoDistribuidos.IsSuccess)
            return residuosNaoDistribuidos.Exception;

        var ordensDeCompra = EmitirOrdensCompra(fechamentos, residuosNaoDistribuidos.Value, cestaVigente.Value, valorTotalConsolidado);

        var ordensCompraEmitidas = await _ordemCompraRepository.SalvarOrdensDeCompra(ordensDeCompra, cancellationToken);

        _logger.LogInformation("Ordens de compra emitidas e registradas. {OrdemCompra}", ordensCompraEmitidas);

        return ordensCompraEmitidas;
    }

    public List<OrdemCompra> EmitirOrdensCompra(Cotacao fechamento, List<CustodiaMaster> residuos, CestaRecomendada cestaVigente, decimal valorTotalConsolidado)
        => fechamento.ComposicaoCotacao.Select(fechamento =>
        {
            var custodia = residuos.FirstOrDefault(x => x.Ticker == fechamento.Ticker);
            var itemCesta = cestaVigente.ComposicaoCesta.FirstOrDefault(x => x.Ticker == fechamento.Ticker)!;

            var qtdNecessariaParaDistribuicao = (int)Math.Truncate(itemCesta.ValorConsolidado(valorTotalConsolidado) / fechamento.PrecoFechamento);

            var quantidadeDeCompraAtivo = custodia?.CalculaNecessidadeLiquidaCompra(qtdNecessariaParaDistribuicao) ?? qtdNecessariaParaDistribuicao;

            return OrdemCompra.GerarOrdemCompra(
                fechamento.Ticker,
                quantidadeDeCompraAtivo,
                fechamento.PrecoFechamento);
        }).ToList();
}