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

    public async Task<Result<List<OrdemCompraDto>>> RegistrarOrdensDeCompraAsync(List<OrdemCompraDto> ordensCompraDto, CancellationToken cancellationToken)
    {
        if (!ordensCompraDto.Any())
            return new ApplicationException("Pelo menos uma ordem de compra deve ser informada para registro");

        var ordensCompra = ordensCompraDto
            .Select(oc => OrdemCompra.GerarOrdemCompra(oc.Ticker, oc.QuantidadeTotal, oc.PrecoUnitario, oc.ValorTotal,
                oc.Detalhes.Select(d => OrdemCompraDetalhe.GerarDetalhes(d.Tipo, d.Ticker, d.Quantidade, 0)).ToList())).ToList();

        var ordensCompraEmitidas = await _ordemCompraRepository.SalvarOrdensDeCompra(ordensCompra, cancellationToken);

        var result = ordensCompraEmitidas
            .Select(oc => new OrdemCompraDto(
                oc.Id,
                oc.Ticker,
                oc.QuantidadeTotal,
                oc.Detalhes.Select(d => new DetalheOrdemCompraDto(d.Tipo, d.Ticker, d.Quantidade)).ToList(),
                oc.PrecoUnitario)).ToList();

        return result;
    }
}