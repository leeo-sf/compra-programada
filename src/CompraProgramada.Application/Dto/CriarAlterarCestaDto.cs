using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Dto;

public class CriarAlterarCestaDto
{
    public required bool CestaAtualizada { get; init; }
    public required CestaRecomendada CestaAtual { get; init; }
    public CestaRecomendada? CestaAnterior { get; init; }
}