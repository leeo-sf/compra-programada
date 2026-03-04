namespace CompraProgramada.Application.Dto;

public class ExecucaoMotorCompraDto
{
    public DateTime DataReferencia { get; set; }
    public DateTime DataExecucao { get; set; }
    public bool Executado { get; set; } = true;
}