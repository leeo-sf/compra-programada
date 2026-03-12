using CompraProgramada.Application.Dto;
using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Response;

public class CriarCestaRecomendadaResponse
{
    public required int CestaId { get; set; }
    public required string Nome { get; set; }
    public required bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public required List<ComposicaoCestaDto> Itens { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CestaDesativadaDto? CestaAnteriorDesativada { get; set; }
    public required bool RebalanceamentoDisparado { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? AtivosRemovidos { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? AtivosAdicionados { get; set; }
    public required string Mensagem { get; set; }
}