using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Dto;

public record DistribuicaoDto
{
    [property: JsonIgnore] public int Id { get; init; }
    [property: JsonIgnore] public string Cpf { get; init; } = string.Empty;
    [property: JsonIgnore] public int OrdemCompraId { get; init; }
    [property: JsonIgnore] public int ContaGraficaId { get; init; }
    [property: JsonIgnore] public string Ticker { get; init; } = string.Empty;
    [property: JsonIgnore] public int QuantidadeAlocada { get; init; }
    [property: JsonIgnore] public decimal ValorOperacao { get; init; }
    [property: JsonIgnore] public ContaGraficaDto ContaGrafica { get; init; } = default!;
    [property: JsonIgnore] public DateTime Data { get; init; }
    public int ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal ValorAporte { get; init; }
    public List<AtivoDto> Ativos { get; init; } = default!;
}