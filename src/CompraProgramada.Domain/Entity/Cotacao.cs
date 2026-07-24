namespace CompraProgramada.Domain.Entity;

public class Cotacao
{
    public int Id { get; }
    public DateOnly DataPregao { get; private set; }
    public DateTime DataCriacao { get; init; }
    public List<ComposicaoCotacao> ComposicaoCotacao { get; init; } = default!;

    private Cotacao() { }

    internal Cotacao(int id, DateOnly dataPregao, DateTime dataCriacao, List<ComposicaoCotacao> composicaoCotacao)
    {
        Id = id;
        DataPregao = dataPregao;
        DataCriacao = dataCriacao;
        ComposicaoCotacao = composicaoCotacao;
    }

    public static Cotacao CriarRegistro(DateOnly dataPregao, List<ComposicaoCotacao> itens)
        => new(0, dataPregao, DateTime.Now, itens);
}