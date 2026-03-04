using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Interface;

public interface IHistoricoExecucaoMotorService
{
    Task<bool> ExecutarCompraHojeAsync(CancellationToken cancellationToken);
    Task SalvarExecucaoAsync(ExecucaoMotorCompraDto execucaoCompra, CancellationToken cancellationToken);
}