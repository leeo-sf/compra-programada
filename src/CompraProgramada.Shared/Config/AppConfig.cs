namespace CompraProgramada.Shared.Config;

public sealed class AppConfig
{
    public MotorCompraConfig MotorCompraConfig { get; init; } = default!;
    public ServiceConfig Service { get; init; } = default!;
}