namespace CompraProgramada.Application.Dto;

public record DetalheOrdemCompraDto(
    string Tipo,
    string Ticker,
    int Quantidade);