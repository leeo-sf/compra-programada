namespace CompraProgramada.Application.Interface;

public interface IMotorCompraService
{
    Task ExecutarCompraAsync(CancellationToken cancellationToken);
}