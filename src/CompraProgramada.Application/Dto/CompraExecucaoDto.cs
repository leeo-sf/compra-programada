namespace CompraProgramada.Application.Dto;

public class CompraExecucaoDto
{
    public DateTime DataReferencia { get; set; }
    public DateTime DataExecucao { get; set; }
    public bool Executado { get; set; } = true;
}