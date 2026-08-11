using CompraProgramada.Domain.Entity.BaseEntity;

namespace CompraProgramada.Domain.Entity;

public class CustodiaMaster : EntidadeBase
{
    public int ContaMasterId { get; init; }
    public string Ticker { get; init; } = default!;
    public int QuantidadeResiduo { get; private set; }
    public decimal PrecoMedio { get; private set; }
    public ContaMaster ContaMaster { get; init; } = default!;

    private CustodiaMaster() { }

    internal CustodiaMaster(int id, DateTime dataCriacao, DateTime dataAtualizacao, int contaMasterId, string ticker, int quantidadeResiduo, ContaMaster contaMaster)
        : base(id, dataCriacao, dataAtualizacao)
    {
        ContaMasterId = contaMasterId;
        Ticker = ticker;
        QuantidadeResiduo = quantidadeResiduo;
        ContaMaster = contaMaster;
    }

    internal CustodiaMaster(int contaMasterId, string ticker)
        : base(0)
    {
        ContaMasterId = contaMasterId;
        Ticker = ticker;
        QuantidadeResiduo = 0;
    }

    public static CustodiaMaster CriarCustodia(int contaMasterId, string ticker)
        => new(contaMasterId, ticker);

    /// <summary>
    /// Identifica quantidade de resíduos restante após distribuição
    /// </summary>
    /// <param name="qtdNovosAtivosComprados">Quantidade de ativos que foram comprados</param>
    /// <param name="qtdUtilizada">Quantidade utilizada na distribuição</param>
    /// <returns>Quantidade de resíduos</returns>
    public int AtualizarResiduo(int qtdNovosAtivosComprados, int qtdUtilizada)
    {
        var qtdDisponivelDistribuicao = QuantidadeResiduo + qtdNovosAtivosComprados;

        QuantidadeResiduo = Math.Max(0, qtdDisponivelDistribuicao - qtdUtilizada);

        return QuantidadeResiduo;
    }

    /// <summary>
    /// Define quantidade de compra com base nos resíduos atuais
    /// </summary>
    /// <param name="quantidadeDesejada">Quantidade necessária para ser distribuída</param>
    /// <returns>Quantidade ativos ah ser comprado</returns>
    public int CalculaNecessidadeLiquidaCompra(int quantidadeDesejada)
    {
        if (QuantidadeResiduo == 0)
            return quantidadeDesejada;
        
        var necessidadeLiquida = Math.Abs(quantidadeDesejada - QuantidadeResiduo);

        return necessidadeLiquida;
    }

    /// <summary>
    /// Calcula e atualiza preço médio ponderado de compra do ativo
    /// </summary>
    /// <param name="precoUnitario">Preço unitário do ativo</param>
    /// <param name="novaQuantidadeDeAtivos">Nova quantidade de ativos que serão atribuídos a conta do cliente</param>
    /// <returns>Valor do preço médio</returns>
    public decimal CalcularPrecoMedio(decimal precoUnitario, int novaQuantidadeDeAtivos)
    {
        if (novaQuantidadeDeAtivos < 1)
            return 0;

        var precoMedioCalculado = (QuantidadeResiduo * PrecoMedio + novaQuantidadeDeAtivos * precoUnitario) / (QuantidadeResiduo + novaQuantidadeDeAtivos);
        PrecoMedio = Math.Round(precoMedioCalculado, 2);

        return PrecoMedio;
    }
}