namespace CompraProgramada.Application.Dto;

public class IRDedoDuroDto
{
    public required int ClienteId { get; set; }
    public required string Cpf { get; set; }
    public required string Ticker { get; set; }
    public required decimal ValorOperacao { get; set; }
    public required decimal ValorIR { get; set; }
    public required DateTime Data { get; set; }
}