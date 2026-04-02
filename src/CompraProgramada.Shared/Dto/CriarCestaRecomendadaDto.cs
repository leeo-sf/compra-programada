namespace CompraProgramada.Shared.Dto;

public class CriarCestaRecomendadaDto
{
    public required bool CestaAtualizada { get; init; }
    public required CestaRecomendadaDto CestaAtual { get; init; }
    public CestaRecomendadaDto? CestaAnterior { get; init; }
}