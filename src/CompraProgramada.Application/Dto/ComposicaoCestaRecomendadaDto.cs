namespace CompraProgramada.Application.Dto;

public record ComposicaoCestaRecomendadaDto(
    int Id,
    int CestaId,
    string Ticker,
    decimal Percentual);