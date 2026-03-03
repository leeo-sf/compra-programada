using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IHistoricoExecucaoMotorRepository
{
    Task<HistoricoExecucaoMotor?> ObtemExecucaoRealizadaAsync(DateTime dataDeExecucao, CancellationToken cancellationToken);
    Task CriarHistoricoExecucaoAsync(HistoricoExecucaoMotor execucao, CancellationToken cancellationToken);
}