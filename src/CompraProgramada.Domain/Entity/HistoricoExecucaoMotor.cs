namespace CompraProgramada.Domain.Entity;

public class HistoricoExecucaoMotor
{
    public int Id { get; init; }
    public DateTime DataReferencia { get; init; }
    public DateTime DataExecucao { get; init; }
    public bool Executado { get; init; }

    private HistoricoExecucaoMotor() { }

    internal HistoricoExecucaoMotor(int id, DateTime dataReferencia, DateTime dataExecucao, bool executado)
    {
        Id = id;
        DataReferencia = dataReferencia;
        DataExecucao = dataExecucao;
        Executado = executado;
    }

    public static HistoricoExecucaoMotor CriarRegistroHistorico(DateTime dataReferencia, DateTime dataExecucao)
        => new HistoricoExecucaoMotor(0, dataReferencia, dataExecucao, true);
}