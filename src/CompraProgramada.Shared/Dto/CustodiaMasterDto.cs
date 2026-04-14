namespace CompraProgramada.Shared.Dto;

public class CustodiaMasterDto
{
    public required string Ticker { get; init; } = string.Empty;
    public required int Quantidade { get; init; }
    public required decimal PrecoMedio { get; init; }
    public required decimal ValorAtual { get; init; }
    public required string Origem { get; init; }
}