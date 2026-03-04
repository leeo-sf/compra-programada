using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface IMotorCompraService
{
    Task ExecutarCompraAsync(CancellationToken cancellationToken);
}