using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class OrdemCompraService : IOrdemCompraService
{
    private readonly IOrdemCompraRepository _ordemCompraRepository;

    public OrdemCompraService(IOrdemCompraRepository ordemCompraRepository) => _ordemCompraRepository = ordemCompraRepository;

    public async Task<Result<List<OrdemCompraDto>>> EmitirOrdensDeCompraAsync(List<OrdemCompraDto> ordensCompraDto, CancellationToken cancellationToken)
    {
        if (ordensCompraDto.Any())
            return new ApplicationException("Pelo menos uma ordem de compra deve ser informada para registro");

        var ordensCompra = ordensCompraDto
            .Where(oc => oc.QuantidadeTotal > 1)
            .Select(oc => new OrdemCompra(0, oc.Ticker, oc.QuantidadeLotePadrao, oc.QuantidadeTotal, oc.PrecoExecucao, DateTime.Now)).ToList();

        var ordensCompraEmitidas = await _ordemCompraRepository.SalvarOrdensDeCompra(ordensCompra, cancellationToken);

        var retorno = ordensCompraEmitidas.Select(oc => new OrdemCompraDto
        {
            Id = oc.Id,
            Ticker = oc.Ticker,
            QuantidadeTotal = oc.Quantidade,
            PrecoExecucao = oc.PrecoExecucao,
            QuantidadeLotePadrao = oc.QuantidadeLotePadrao
        }).ToList();

        return retorno;
    }
}