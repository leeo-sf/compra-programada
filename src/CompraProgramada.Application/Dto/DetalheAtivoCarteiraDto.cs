namespace CompraProgramada.Application.Dto;

public record DetalheAtivoCarteiraDto(
    string Ticker,
    int Quantidade,
    decimal PrecoMedio,
    decimal CotacaoAtual,
    decimal ValorAtual,
    decimal Pl,
    decimal PlPercentual,
    decimal ComposicaoCarteira);