namespace CompraProgramada.Application.Dto;

public class CestaRecomandadaDto
{
    public required int Id { get; init; }
    public required string Nome { get; init; }
    public required DateTime DataCriacao { get; init; }
    public DateTime DataDesativacao { get; init; }
    public bool Ativa { get; init; } = true;
    public List<ComposicaoCestaRecomendadaDto> Itens { get; init; } = new List<ComposicaoCestaRecomendadaDto>();
}