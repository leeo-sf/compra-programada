namespace CompraProgramada.Application.Config;

public sealed class MotorCompra
{
    public int[] DiasDeCompra { get; set; } = default!;
    public int TempoEmHoraAhCadaExecucao { get; set; }
}