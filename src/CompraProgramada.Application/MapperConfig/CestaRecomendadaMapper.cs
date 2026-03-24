using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CestaRecomendadaMapper
{
    [MapProperty(nameof(CestaRecomendada.ComposicaoCesta), nameof(CestaRecomendadaDto.Itens))]
    public partial CestaRecomendadaDto ToResponse(CestaRecomendada cesta);
    public partial List<CestaRecomendadaDto> ToResponse(List<CestaRecomendada> cesta);


    [MapProperty(nameof(CestaRecomendadaDto.Id), nameof(CestaRecomendadaResponse.CestaId))]
    public partial CestaRecomendadaResponse ToResponse(CestaRecomendadaDto cesta);
    public partial List<CestaRecomendadaResponse> ToResponse(List<CestaRecomendadaDto> cesta);

    
    public partial ComposicaoCestaRecomendadaDto ToResponse(ComposicaoCesta cesta);
    public partial ComposicaoCestaDto ToResponse(ComposicaoCestaRecomendadaDto cesta);
}