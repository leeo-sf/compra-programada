using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public record ContaGraficaResponse(
    int Id,
    string NumeroConta,
    string Tipo,
    DateTime DataCriacao);