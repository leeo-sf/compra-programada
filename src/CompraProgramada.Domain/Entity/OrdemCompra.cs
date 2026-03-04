using CompraProgramada.Domain.Enum;

namespace CompraProgramada.Domain.Entity;

public record OrdemCompra(int Id, string Ticker, int Quantidade, decimal PrecoExecucao, DateTime Data, TipoMercado TipoMercado)
{
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
}