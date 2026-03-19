using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
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
    private readonly OrdemCompraMapper _mapper;

    public OrdemCompraService(ILogger<OrdemCompraService> logger,
        IOrdemCompraRepository ordemCompraRepository,
        ICotacaoService cotacaoService,
        ICustodiaMasterService custodiaMasterService,
        OrdemCompraMapper mapper)
    {
        _logger = logger;
        _ordemCompraRepository = ordemCompraRepository;
        _cotacaoService = cotacaoService;
        _custodiaMasterService = custodiaMasterService;
        _mapper = mapper;
    }

    public async Task<Result<List<OrdemCompraDto>>> EmitirOrdensDeCompraAsync(decimal valorTotalConsolidado, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando emissão de ordens de compra...");

        var fechamentosResult = await _cotacaoService.ObterCombinacoesFechamentoECompraAtivoAsync(valorTotalConsolidado, cancellationToken);
        if (!fechamentosResult.IsSuccess)
            throw fechamentosResult.Exception;

        var fechamentos = fechamentosResult.Value;

        _logger.LogInformation("Fechamento dos ativos correspondentes a cesta atual obtidos: {Fechamentos}", fechamentos);

        var residuosNaoDistribuidos = await _custodiaMasterService.ObterResiduosNaoDistribuidos(cancellationToken);
        if (!residuosNaoDistribuidos.IsSuccess)
            throw residuosNaoDistribuidos.Exception;

        var ordensDeCompra = new List<OrdemCompra>();
        foreach (var fechamento in fechamentos)
        {
            var custodia = residuosNaoDistribuidos.Value.FirstOrDefault(x => x.Ticker == fechamento.Ticker);

            var qtdNecessariaParaDistribuicao = (int)Math.Truncate(fechamento.ValorAhCompra / fechamento.PrecoFechamento);

            var quantidadeDeCompraAtivo = _custodiaMasterService.SubtrairResiduosParaCompra(custodia, qtdNecessariaParaDistribuicao);

            ordensDeCompra.Add(OrdemCompra.GerarOrdemCompra(
                fechamento.Ticker,
                quantidadeDeCompraAtivo,
                fechamento.PrecoFechamento));
        }

        var ordensCompraEmitidas = await _ordemCompraRepository.SalvarOrdensDeCompra(ordensDeCompra, cancellationToken);
        var ordensCompraEmitidasDto = _mapper.ToResponse(ordensCompraEmitidas);

        _logger.LogInformation("Ordens de compra emitidas e registradas. {OrdemCompra}", ordensCompraEmitidasDto);

        return ordensCompraEmitidasDto;
    }

    public async Task<Result<List<OrdemCompraDto>>> ObterOrdemCompraAsync(DateTime dataEmissao, CancellationToken cancellationToken)
    {
        var ordensCompra = await _ordemCompraRepository.ObterOrdensCompraAsync(dataEmissao, cancellationToken);
        if (ordensCompra is null)
            return new ApplicationException($"Nenhuma ordem de compra registrada na data {dataEmissao:dd/MM/yyyy}.");

        return _mapper.ToResponse(ordensCompra);
    }
}