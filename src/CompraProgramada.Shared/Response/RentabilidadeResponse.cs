using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public record RentabilidadeResponse(
    int ClienteId,
    string Nome,
    DateTime DataConsulta,
    ResumoCarteiraDto Rentabilidade,
    List<HistoricoAporteDto> HistoricoAportes,
    List<EvolucaoCarteiraDto> EvolucaoCarteira);