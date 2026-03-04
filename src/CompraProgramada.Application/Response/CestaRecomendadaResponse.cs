using CompraProgramada.Application.Dto;
using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Response;

public class CestaRecomendadaResponse
{
    public required int CestaId { get; set; }
    public required string Nome { get; set; }
    public required bool Ativa { get; set; }
    public required DateTime DataCriacao { get; set; }
    public required List<ComposicaoCestaDto> Itens { get; set; }
}