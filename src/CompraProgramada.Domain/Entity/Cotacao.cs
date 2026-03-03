namespace CompraProgramada.Domain.Entity;

public record Cotacao(int Id, DateTime DataPregao)
{
    public List<ComposicaoCotacao> ComposicaoCotacao { get; init; } = default!;
}