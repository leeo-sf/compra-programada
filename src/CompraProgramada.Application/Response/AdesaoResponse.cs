using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public class AdesaoResponse
{
    public required int ClienteId { get; set; }
    public required string Nome { get; set; }
    public required string Cpf { get; set; }
    public required string Email { get; set; }
    public required decimal ValorMensal { get; set; }
    public required bool Ativo { get; set; }
    public required DateTime DataAdesao { get; set; }
    public required ContaGraficaDto ContaGrafica { get; set; }
}