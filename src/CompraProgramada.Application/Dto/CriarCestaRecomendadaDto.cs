namespace CompraProgramada.Application.Dto;

public record CriarCestaRecomendadaDto(
    bool CestaAtualizada,
    CestaRecomandadaDto CestaAtual,
    CestaRecomandadaDto? CestaAnterior);