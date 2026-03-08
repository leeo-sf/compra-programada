namespace CompraProgramada.Application.Dto;

public record HistoricoCompraDto(
    int ContaGraficaId,
    string Ticker,
    int Quantidade,
    decimal ValorAporte,
    decimal PrecoExecutado,
    decimal PrecoMedio,
    DateOnly Data);