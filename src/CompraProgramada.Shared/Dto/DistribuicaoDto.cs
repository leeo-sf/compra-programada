using System.Text.Json.Serialization;

namespace CompraProgramada.Shared.Dto;

public record DistribuicaoDto
{
    [JsonIgnore]
    public int Id { get; init; }
    [JsonIgnore]
    public string Cpf { get; init; } = string.Empty;
    [JsonIgnore]
    public int OrdemCompraId { get; init; }
    [JsonIgnore]
    public int ContaGraficaId { get; init; }
    [JsonIgnore]
    public string Ticker { get; init; } = string.Empty;
    [JsonIgnore]
    public int QuantidadeAlocada { get; init; }
    [JsonIgnore]
    public decimal ValorOperacao { get; init; }
    [JsonIgnore]
    public ContaGraficaDto ContaGrafica { get; init; } = default!;
    [JsonIgnore]
    public DateTime Data { get; init; }
    public int ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal ValorAporte { get; init; }
    public List<AtivoQuantidadeDto> Ativos { get; init; } = default!;
}