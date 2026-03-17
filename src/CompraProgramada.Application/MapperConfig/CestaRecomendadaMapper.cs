using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CestaRecomendadaMapper
{
    [MapProperty(nameof(CestaRecomendada.ComposicaoCesta), nameof(CestaRecomandadaDto.Itens))]
    public partial CestaRecomandadaDto ToResponse(CestaRecomendada cesta);
    public partial List<CestaRecomandadaDto> ToResponse(List<CestaRecomendada> cesta);


    [MapProperty(nameof(CestaRecomandadaDto.Id), nameof(CestaRecomendadaResponse.CestaId))]
    public partial CestaRecomendadaResponse ToResponse(CestaRecomandadaDto cesta);
    public partial List<CestaRecomendadaResponse> ToResponse(List<CestaRecomandadaDto> cesta);

    
    public partial ComposicaoCestaRecomendadaDto ToResponse(ComposicaoCesta cesta);
    public partial ComposicaoCestaDto ToResponse(ComposicaoCestaRecomendadaDto cesta);
}