namespace CompraProgramada.Application.Dto;

public record CustodiaFilhoteDto(
    int Id,
    int ContaGraficaId,
    string Ticker,
    decimal PrecoMedio,
    int Quantidade);