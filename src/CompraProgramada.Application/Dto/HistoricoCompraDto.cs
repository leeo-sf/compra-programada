namespace CompraProgramada.Application.Dto;

public record HistoricoCompraDto(
    int Id,
    decimal Valor,
    DateOnly Data,
    int ContaGraficaId);