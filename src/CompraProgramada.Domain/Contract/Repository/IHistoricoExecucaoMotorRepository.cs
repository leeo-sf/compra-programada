using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IHistoricoExecucaoMotorRepository
{
    Task<HistoricoExecucaoMotor?> ObterHistoricoExecucaoAsync(DateTime dataDeExecucao, CancellationToken cancellationToken);
    Task SalvarHistoricoExecucaoAsync(HistoricoExecucaoMotor execucao, CancellationToken cancellationToken);
}