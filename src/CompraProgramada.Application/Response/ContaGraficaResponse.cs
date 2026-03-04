using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public class ContaGraficaResponse
{
    public required int Id { get; set; }
    public required string NumeroConta { get; set; }
    public required string Tipo { get; set; }
    public required DateTime DataCriacao { get; set; }
}