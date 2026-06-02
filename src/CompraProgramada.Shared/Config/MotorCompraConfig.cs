namespace CompraProgramada.Shared.Config;

public sealed class MotorCompraConfig
{
    public int[] DiasDeCompra { get; set; } = default!;
    public int TempoEmHoraAhCadaExecucao { get; set; }
    public string NomePastaArquivosB3 { get; set; } = default!;
}