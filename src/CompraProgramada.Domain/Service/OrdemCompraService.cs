using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Domain.Service;

public class OrdemCompraService : IOrdemCompraService
{
    private readonly ILogger<OrdemCompraService> _logger;
    private readonly IOrdemCompraRepository _ordemCompraRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICustodiaMasterRepository _custodiaMasterRepository;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;

    public OrdemCompraService(
        ILogger<OrdemCompraService> logger,
        IOrdemCompraRepository ordemCompraRepository,
        ICotacaoService cotacaoService,
        ICustodiaMasterRepository custodiaMasterRepository,
        ICestaRecomendadaRepository cestaRecomendadaRepository)
    {
        _logger = logger;
        _ordemCompraRepository = ordemCompraRepository;
        _cotacaoService = cotacaoService;
        _custodiaMasterRepository = custodiaMasterRepository;
        _cestaRecomendadaRepository = cestaRecomendadaRepository;
    }

    public async Task<Result<List<OrdemCompra>>> EmitirOrdensDeCompraAsync(decimal valorTotalConsolidado, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando emissão de ordens de compra...");

        var cestaVigente = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);
        if (cestaVigente is null)
            return new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        var fechamentosResult = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente, cancellationToken);
        if (!fechamentosResult.IsSuccess)
            return fechamentosResult.Exception;

        var fechamentos = fechamentosResult.Value;

        _logger.LogInformation("Fechamento dos ativos correspondentes a cesta atual obtidos: {Fechamentos}", fechamentos);

        var residuos = await _custodiaMasterRepository.ObterResiduosAsync(cancellationToken);

        List<OrdemCompra> ordensCompra = [];

        foreach (var fechamento in fechamentos.ComposicaoCotacao)
        {
            var custodia = residuos?.FirstOrDefault(x => x.Ticker == fechamento.Ticker);
            var ativoCesta = cestaVigente.ComposicaoCesta.FirstOrDefault(x => x.Ticker == fechamento.Ticker);

            if (ativoCesta is null)
            {
                _logger.LogWarning("Ativo {Ticker} não encontrado na composição da cesta vigente", fechamento.Ticker);
                continue;
            }

            var qtdNecessariaParaDistribuicao = (int)Math.Truncate(ativoCesta.ValorConsolidado(valorTotalConsolidado) / fechamento.PrecoFechamento);
            var quantidadeDeCompraAtivo = custodia?.CalculaNecessidadeLiquidaCompra(qtdNecessariaParaDistribuicao) ?? qtdNecessariaParaDistribuicao;

            var ordemCompra = OrdemCompra.GerarOrdemCompra(fechamento.Ticker, quantidadeDeCompraAtivo, fechamento.PrecoFechamento);
            ordensCompra.Add(ordemCompra);
        }

        var ordensCompraEmitidas = await _ordemCompraRepository.SalvarOrdensCompraAsync(ordensCompra, cancellationToken);

        _logger.LogInformation("Ordens de compra emitidas e registradas. {OrdemCompra}", ordensCompraEmitidas);

        return ordensCompraEmitidas;
    }
}