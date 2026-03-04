namespace CompraProgramada.Application.Dto;

public class MotorDto
{
    public decimal TotalConsolidado { get; set; }
    public List<ClienteDto> Clientes { get; set; } = default!;
    public List<AtivoDto> Acoes { get; set; } = default!;
}