namespace CompraProgramada.Domain.Entity;

public record CestaRecomendada(int Id, string Nome, DateTime DataCriacao, DateTime DataDesativacao, bool Ativa = true)
{
    public int Id { get; init; } = Id;
    public string Nome { get; init; } = Nome;
    public DateTime DataCriacao { get; init; } = DataCriacao;
    public DateTime DataDesativacao { get; init; } = DataDesativacao;
    public bool Ativa { get; init; } = Ativa;
    public List<ComposicaoCesta> ComposicaoCesta { get; init; } = default!;
}