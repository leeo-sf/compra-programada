namespace CompraProgramada.Application.Dto;

public record ContaGraficaDto
{
    public required int Id { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public required DateTime DataCriacao { get; set; }
    public required int ClienteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public List<CustodiaFilhoteDto>? CustodiaFilhote { get; set; }
}