using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public class CestaRecomendadaResponse
{
    public required int CestaId { get; set; }
    public required string Nome { get; set; }
    public required bool Ativa { get; set; }
    public required DateTime DataCriacao { get; set; }
    public required List<ComposicaoCestaDto> Itens { get; set; }
}