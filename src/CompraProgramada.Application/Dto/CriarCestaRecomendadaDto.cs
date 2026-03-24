namespace CompraProgramada.Application.Dto;

public record CriarCestaRecomendadaDto(
    bool CestaAtualizada,
    CestaRecomendadaDto CestaAtual,
    CestaRecomendadaDto? CestaAnterior);