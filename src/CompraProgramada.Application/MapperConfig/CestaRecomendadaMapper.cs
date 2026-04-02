using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Response;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CestaRecomendadaMapper
{
    [MapProperty(nameof(CestaRecomendada.Id), nameof(CestaRecomendadaDto.CestaId))]
    [MapProperty(nameof(CestaRecomendada.ComposicaoCesta), nameof(CestaRecomendadaDto.Itens))]
    public partial CestaRecomendadaDto ToResponse(CestaRecomendada cesta);
    public partial List<CestaRecomendadaDto> ToResponse(List<CestaRecomendada> cesta);


    public partial CestaRecomendadaResponse ToResponse(CestaRecomendadaDto cesta);
    public partial List<CestaRecomendadaResponse> ToResponse(List<CestaRecomendadaDto> cesta);

    
    public partial ComposicaoCestaDto ToResponse(ComposicaoCesta cesta);
}