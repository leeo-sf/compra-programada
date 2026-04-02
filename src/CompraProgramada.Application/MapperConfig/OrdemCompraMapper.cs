using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class OrdemCompraMapper
{
    public partial OrdemCompraDto ToResponse(OrdemCompra ordemCompra);
    public partial List<OrdemCompraDto> ToResponse(List<OrdemCompra> ordensCompra);


    public partial OrdemCompraDetalheDto ToResponse(OrdemCompraDetalhe ordemCompraDetalhe);
    public partial List<OrdemCompraDetalheDto> ToResponse(List<OrdemCompraDetalhe> ordemCompraDetalhes);
}