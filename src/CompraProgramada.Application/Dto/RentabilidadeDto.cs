namespace CompraProgramada.Application.Dto;

public record RentabilidadeDto(
    List<HistoricoAporteDto> HistoricoAportes,
    List<EvolucaoCarteiraDto> EvolucaoCarteira);