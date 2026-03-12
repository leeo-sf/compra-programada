namespace CompraProgramada.Application.Dto;

public record ClienteDto(
    int ClienteId,
    string Nome,
    string Cpf,
    string Email,
    decimal ValorAnterior,
    decimal ValorMensal,
    bool Ativo,
    DateTime DataAdesao,
    ContaGraficaDto ContaGrafica)
{
    public decimal ValorAporte => ValorMensal / 3;
}