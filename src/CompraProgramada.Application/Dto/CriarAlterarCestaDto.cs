namespace CompraProgramada.Application.Dto;

public class CriarAlterarCestaDto
{
    public required bool CestaAtualizada { get; init; }
    public required CestaRecomandadaDto CestaAtual { get; init; }
    public CestaRecomandadaDto? CestaAnterior { get; init; }
}