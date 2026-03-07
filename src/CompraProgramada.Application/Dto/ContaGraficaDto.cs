namespace CompraProgramada.Application.Dto;

public record ContaGraficaDto(
    int Id,
    string NumeroConta,
    DateTime? DataCriacao,
    int ClienteId,
    string Tipo,
    List<HistoricoCompraDto>? HistoricoCompra,
    List<CustodiaFilhoteDto> CustodiaFilhotes);