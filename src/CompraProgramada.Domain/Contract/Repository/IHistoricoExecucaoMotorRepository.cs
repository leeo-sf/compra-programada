using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IHistoricoExecucaoMotorRepository
{
    Task<HistoricoExecucaoMotor?> ObtemExecucaoRealizadaAsync(DateTime dataDeExecucao, CancellationToken cancellationToken);
    Task CriarHistoricoExecucaoAsync(HistoricoExecucaoMotor execucao, CancellationToken cancellationToken);
}