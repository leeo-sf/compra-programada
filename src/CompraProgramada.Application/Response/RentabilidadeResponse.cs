using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public record RentabilidadeResponse(
    int ClienteId,
    string Nome,
    DateTime DataConsulta,
    ResumoCarteiraDto Rentabilidade,
    List<HistoricoAporteDto> HistoricoAportes,
    List<EvolucaoCarteiraDto> EvolucaoCarteira);