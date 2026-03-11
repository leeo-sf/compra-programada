using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public record AdesaoResponse(
    int ClienteId,
    string Nome,
    string Cpf,
    string Email,
    decimal ValorMensal,
    bool Ativo,
    DateTime DataAdesao,
    ContaGraficaResponse ContaGrafica);