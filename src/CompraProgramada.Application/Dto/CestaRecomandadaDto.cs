namespace CompraProgramada.Application.Dto;

public record CestaRecomandadaDto(
    int Id,
    string Nome,
    DateTime DataCriacao,
    DateTime? DataDesativacao,
    bool Ativa,
    List<ComposicaoCestaRecomendadaDto> Itens);