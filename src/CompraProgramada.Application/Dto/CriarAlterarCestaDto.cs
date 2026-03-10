namespace CompraProgramada.Application.Dto;

public record CriarAlterarCestaDto(
    bool CestaAtualizada,
    CestaRecomandadaDto CestaAtual,
    CestaRecomandadaDto? CestaAnterior);