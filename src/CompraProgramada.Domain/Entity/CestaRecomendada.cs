using CompraProgramada.Shared.Exceptions;

namespace CompraProgramada.Domain.Entity;

public class CestaRecomendada
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = default!;
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataDesativacao { get; private set; }
    public bool Ativa { get; private set; }
    public List<ComposicaoCesta> ComposicaoCesta { get; private set; } = default!;
    private const int QUANTIDADE_EXATA_ITENS_CESTA = 5;
    private const int SOMA_PERCENTUAIS_EXATA = 100;

    private CestaRecomendada() { }

    internal CestaRecomendada(int id, string nome, DateTime dataCriacao, DateTime? dataDesativacao, bool ativa, List<ComposicaoCesta> itens)
    {
        Id = id;
        Nome = nome;
        DataCriacao = dataCriacao;
        DataDesativacao = dataDesativacao;
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

        return new CestaRecomendada(0, nome.ToUpper(), DateTime.Now, null, true, itens);
    }

    public void DesativarCesta()
    {
        if (!Ativa)
            throw new ApplicationException("A cesta já se encontra desativada.");

        Ativa = false;
        DataDesativacao = DateTime.Now;
    }
}