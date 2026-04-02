namespace CompraProgramada.Shared.Dto;

public class CotacaoB3Dto
{
    public DateTime DataPregao { get; init; }
    public string Ticker { get; init; } = string.Empty;
    public string CodigoBDI { get; init; } = string.Empty;
    public int TipoMercado { get; init; }
    public string NomeEmpresa { get; init; } = string.Empty;
    public decimal PrecoAbertura { get; init; }
    public decimal PrecoMaximo { get; init; }
    public decimal PrecoMinimo { get; init; }
    public decimal PrecoFechamento { get; init; }
    public decimal PrecoMedio { get; init; }
    public long QuantidadeNegociada { get; init; }
    public decimal VolumeNegociado { get; init; }
}