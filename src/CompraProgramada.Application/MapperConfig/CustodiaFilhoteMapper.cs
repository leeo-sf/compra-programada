using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CustodiaFilhoteMapper
{
    public partial CustodiaFilhoteDto ToResponse(CustodiaFilhote custodia);
    public partial List<CustodiaFilhoteDto> ToResponse(List<CustodiaFilhote> custodias);
}