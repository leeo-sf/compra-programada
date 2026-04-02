namespace CompraProgramada.Shared.Dto;

public class IRDedoDuroDto
{
    public required int ClienteId { get; init; }
    public required string Cpf { get; init; }
    public required string Ticker { get; init; }
    public required decimal ValorOperacao { get; init; }
    public required decimal ValorIR { get; init; }
    public required DateTime Data { get; init; }
}