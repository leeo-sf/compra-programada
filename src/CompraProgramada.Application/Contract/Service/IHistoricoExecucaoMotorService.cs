namespace CompraProgramada.Application.Contract.Service;

public interface IHistoricoExecucaoMotorService
{
    Task<bool> ExecutarCompraHojeAsync(CancellationToken cancellationToken);
    Task SalvarExecucaoAsync(DateTime dataReferencia, DateTime dataExecucao, CancellationToken cancellationToken);
}