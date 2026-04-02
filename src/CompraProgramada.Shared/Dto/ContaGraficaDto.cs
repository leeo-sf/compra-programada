namespace CompraProgramada.Shared.Dto;

public class ContaGraficaDto
{
    public required int Id { get; init;  }
    public required string NumeroConta { get; init; } = string.Empty;
    public required DateTime DataCriacao { get; init; }
    public required int ClienteId { get; init; }
    public required string Tipo { get; init; } = string.Empty;
    public List<HistoricoCompraDto>? HistoricoCompra { get; init; }
    public required List<CustodiaFilhoteDto> CustodiaFilhotes { get; init; }
}