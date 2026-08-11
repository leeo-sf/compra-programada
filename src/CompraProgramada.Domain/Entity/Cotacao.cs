using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public class Cotacao : EntidadeBase
{
    public DateOnly DataPregao { get; init; }
    public List<ComposicaoCotacao> ComposicaoCotacao { get; init; } = default!;

    private Cotacao() { }

    internal Cotacao(DateOnly dataPregao, List<ComposicaoCotacao> composicaoCotacao)
        : base(0)
    {
        DataPregao = dataPregao;
        ComposicaoCotacao = composicaoCotacao;
    }

    public static Cotacao CriarRegistro(DateOnly dataPregao, List<ComposicaoCotacao> itens)
        => new(dataPregao, itens);
}