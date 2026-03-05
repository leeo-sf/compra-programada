namespace CompraProgramada.Domain.Entity;

public record OrdemCompra(int Id, string Ticker, int QuantidadeLotePadrao, int Quantidade, decimal PrecoExecucao, DateTime Data)
{
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
}