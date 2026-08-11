using CompraProgramada.Domain.Entity.BaseEntity;
using CompraProgramada.Shared.Exceptions;

namespace CompraProgramada.Domain.Entity;

public class CestaRecomendada : EntidadeBase
{
    public string Nome { get; private set; } = default!;
    public bool Ativa { get; private set; }
    public List<ComposicaoCesta> ComposicaoCesta { get; private set; } = default!;
    private const int QUANTIDADE_EXATA_ITENS_CESTA = 5;
    private const int SOMA_PERCENTUAIS_EXATA = 100;

    private CestaRecomendada() { }

    internal CestaRecomendada(string nome, bool ativa, List<ComposicaoCesta> itens)
        : base(0)
    {
        Nome = nome;
        Ativa = ativa;
        ComposicaoCesta = itens;
    }

    public static CestaRecomendada CriarCesta(string nome, List<ComposicaoCesta> itens)
    {
        if (itens.Count != QUANTIDADE_EXATA_ITENS_CESTA)
            throw new QuantidadeItensCestaException(itens.Count);

        var somaPercentuaisAtivos = itens.Sum(item => item.Percentual);
        if (somaPercentuaisAtivos != SOMA_PERCENTUAIS_EXATA)
            throw new PercentualCestaException(somaPercentuaisAtivos);

        return new(nome.ToUpper(), true, itens);
    }

    public void DesativarCesta()
    {
        if (!Ativa)
            throw new ApplicationException("A cesta já se encontra desativada.");

        Ativa = false;
        DataAtualizacao = DateTime.UtcNow;
    }
}