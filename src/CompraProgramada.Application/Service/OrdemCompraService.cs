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

        var ordensCompra = EmitirOrdensDeCompra(ordensCompraDto);

        var ordensCompraEmitidas = await _ordemCompraRepository.SalvarOrdensDeCompra(ordensCompra, cancellationToken);

        var retorno = ordensCompraEmitidas.Select(oc => new OrdemCompraDto
        {
            Id = oc.Id,
            Ticker = oc.Ticker,
            QuantidadeCompra = oc.Quantidade,
            PrecoExecucao = oc.PrecoExecucao
        }).ToList();

        return retorno;
    }

    public List<OrdemCompra> EmitirOrdensDeCompra(List<OrdemCompraDto> ordensCompraDto)
        => ordensCompraDto.Select(ordem => new OrdemCompra(0, ordem.Ticker, ordem.QuantidadeCompra / 100, ordem.QuantidadeCompra, ordem.PrecoExecucao, DateTime.Now)).ToList();
}