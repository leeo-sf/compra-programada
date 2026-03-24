using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class HistoricoCompraMapper
{
    public partial HistoricoCompraDto ToResponse(HistoricoCompra historico);
    public partial List<HistoricoCompraDto> ToResponse(List<HistoricoCompra> historico);
}