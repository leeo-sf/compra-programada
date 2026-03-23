namespace CompraProgramada.Domain.Entity;

public class CustodiaMaster
{
    public int Id { get; init; }
    public int ContaMasterId { get; init; }
    public string Ticker { get; init; } = default!;
    public int QuantidadeResiduo { get; private set; }
    public ContaMaster ContaMaster { get; init; } = default!;

    private CustodiaMaster() { }

    internal CustodiaMaster(int id, int contaMasterId, string ticker, int quantidadeResiduo, ContaMaster contaMaster)
    {
        Id = id;
        ContaMasterId = contaMasterId;
        Ticker = ticker;
        QuantidadeResiduo = quantidadeResiduo;
        ContaMaster = contaMaster;
    }

    internal CustodiaMaster(int contaMasterId, string ticker)
    {
        Id = 0;
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

        var residuo = Math.Max(0, qtdDisponivelDistribuicao - qtdUtilizada);

        QuantidadeResiduo = residuo;

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
        
        var necessidadeLiquida = quantidadeDesejada - QuantidadeResiduo;

        if (necessidadeLiquida < 0)
            necessidadeLiquida = QuantidadeResiduo - quantidadeDesejada;

        return necessidadeLiquida;
    }
}