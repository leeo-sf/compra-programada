using CompraProgramada.Application.Dto;
using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Response;

public record CriarCestaRecomendadaResponse(
    int CestaId,
    string Nome,
    bool Ativa,
    DateTime DataCriacao,
    List<ComposicaoCestaDto> Itens,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] CestaDesativadaDto? CestaAnteriorDesativada,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<string>? AtivosRemovidos,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<string>? AtivosAdicionados,
    bool RebalanceamentoDisparado = false,
    string Mensagem = "Primeira cesta cadastrada com sucesso.");