using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Dto;

public record CestaRecomendadaDto(
    int Id,
    string Nome,
    DateTime DataCriacao,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] DateTime? DataDesativacao,
    bool Ativa,
    List<ComposicaoCestaRecomendadaDto> Itens);