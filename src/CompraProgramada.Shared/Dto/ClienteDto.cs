namespace CompraProgramada.Shared.Dto;

public class ClienteDto
{
    public required int ClienteId { get; init; }
    public required string Nome { get; init; }
    public required string Cpf { get; init; }
    public required string Email { get; init; }
    public required decimal ValorAnterior { get; init; }
    public required decimal ValorMensal { get; init; }
    public required bool Ativo { get; init; }
    public required DateTime DataAdesao { get; init; }
    public required ContaGraficaDto ContaGrafica { get; init; }
    public required decimal ValorAporte { get; init; }
}