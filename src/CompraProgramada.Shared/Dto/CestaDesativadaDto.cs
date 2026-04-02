namespace CompraProgramada.Shared.Dto;

public class CestaDesativadaDto
{
    public required int CestaId { get; init; }
    public required string Nome { get; init; } = string.Empty;
    public required DateTime DataDesativacao { get; init; }
}