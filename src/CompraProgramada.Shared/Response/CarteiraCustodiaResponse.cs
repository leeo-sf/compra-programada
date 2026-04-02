using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public record CarteiraCustodiaResponse(
    int ClienteId,
    string Nome,
    string ContaGrafica,
    DateTime DataConsulta,
    ResumoCarteiraDto Resumo,
    List<DetalheCarteiraDto> Ativos);