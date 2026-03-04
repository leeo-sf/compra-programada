namespace CompraProgramada.Application.Dto;

public class ClienteDto
{
    public required int ClienteId { get; set; }
    public required string Nome { get; set; }
    public required string Cpf { get; set; }
    public required string Email { get; set; }
    public required decimal ValorAnterior { get; set; }
    public required decimal ValorMensal { get; set; }
    public required bool Ativo { get; set; }
    public required DateTime DataAdesao { get; set; }
    public required List<AtivoDto> Ativos { get; set; }
    public decimal ValorAporte => ValorMensal / 3;
}