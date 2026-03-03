namespace CompraProgramada.Application.Dto;

public class CestaDesativadaDto
{
    public required int CestaId { get; set; }
    public required string Nome { get; set; }
    public required DateTime DataDesativacao { get; set; }
}