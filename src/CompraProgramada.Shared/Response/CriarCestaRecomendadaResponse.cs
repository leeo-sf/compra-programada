using CompraProgramada.Shared.Dto;
using System.Text.Json.Serialization;

namespace CompraProgramada.Shared.Response;

public record CriarCestaRecomendadaResponse(
    int CestaId,
    string Nome,
    bool Ativa,
    DateTime DataCriacao,
    List<ComposicaoCestaDto> Itens,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] CestaDesativadaDto? CestaAnteriorDesativada,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<string>? AtivosRemovidos,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<string>? AtivosAdicionados,
    bool RebalanceamentoDisparado,
    string Mensagem);