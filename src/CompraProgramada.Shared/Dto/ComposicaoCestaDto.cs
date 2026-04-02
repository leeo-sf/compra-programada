using System.Text.Json.Serialization;

namespace CompraProgramada.Shared.Dto;

public class ComposicaoCestaDto
{
    public required string Ticker { get; init; }
    public required decimal Percentual { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CotacaoAtual { get; init; }
}