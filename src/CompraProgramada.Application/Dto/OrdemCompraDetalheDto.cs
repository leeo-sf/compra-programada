namespace CompraProgramada.Application.Dto;

public record OrdemCompraDetalheDto(
    string Tipo,
    string Ticker,
    int Quantidade);