using System.Text.Json.Serialization;

namespace CompraProgramada.Shared.Dto;

public class CestaRecomendadaDto
{
    public required int CestaId { get; init; }
    public required string Nome { get; init; }
    public required DateTime DataCriacao { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public DateTime? DataDesativacao { get; init; }
    public required bool Ativa { get; init; }
    public required List<ComposicaoCestaDto> Itens { get; init; }
}