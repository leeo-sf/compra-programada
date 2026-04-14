namespace CompraProgramada.Shared.Dto;

public class ContaMasterDto
{
    public required int Id { get; init; }
    public required string NumeroConta { get; init; } = string.Empty;
    public required string Tipo { get; init; } = string.Empty;
}