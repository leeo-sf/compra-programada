namespace CompraProgramada.Domain.Entity;

public class Cotacao
{
    public int Id { get; init; }
    public DateTime DataPregao { get; init; }
    public List<ComposicaoCotacao> ComposicaoCotacao { get; init; } = default!;

    private Cotacao() { }

    internal Cotacao(int id, DateTime dataPregao, List<ComposicaoCotacao> composicaoCotacao)
    {
        Id = id;
        DataPregao = dataPregao;
        ComposicaoCotacao = composicaoCotacao;
    }

    public static Cotacao CriarRegistro(DateTime dataPregao, List<ComposicaoCotacao> itens)
        => new(0, dataPregao, itens);
}