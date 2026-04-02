namespace CompraProgramada.Shared.Dto;

public class DetalheCarteiraDto
{
    public required string Ticker { get; set; }
    public required int Quantidade { get; set; }
    public required decimal PrecoMedio { get; set; }
    public required decimal CotacaoAtual { get; set; }
    public required decimal ValorAtual { get; set; }
    public required decimal Pl { get; set; }
    public required decimal PlPercentual { get; set; }
    public required decimal ComposicaoCarteira { get; set; }
}