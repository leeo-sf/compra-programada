namespace CompraProgramada.Domain.Entity;

public record HistoricoExecucaoMotor(int Id, DateTime DataReferencia, DateTime DataExecucao, bool Executado = true);