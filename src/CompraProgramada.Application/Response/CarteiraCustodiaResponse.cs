using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public record CarteiraCustodiaResponse(
    int ClienteId,
    string Nome,
    string ContaGrafica,
    DateTime DataConsulta,
    ResumoCarteiraDto resumo,
    List<DetalheAtivoCarteiraDto> Ativos);