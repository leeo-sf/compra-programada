using CompraProgramada.Domain.Entity.BaseEntity;
using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Domain.Entity;

public class ContaGrafica : BaseConta
{
    public int ClienteId { get; init; }
    public string Tipo { get; } = "FILHOTE";
    public Cliente Cliente { get; private set; } = default!;
    public List<Distribuicao> Distribuicoes { get; init; } = new List<Distribuicao>();
    public List<CustodiaFilhote> CustodiaFilhotes { get; init; } = new List<CustodiaFilhote>();
    public List<HistoricoCompra> HistoricoCompra { get; init; } = new List<HistoricoCompra>();
    private const int NUMERO_PARCELA_MAXIMO_COMPRA = 3;

    private ContaGrafica() : base() { }

    internal ContaGrafica(int id, string numeroConta, DateTime dataCriacao, Cliente cliente, List<Distribuicao> distribuicoes, List<CustodiaFilhote> custodiaFilhotes, List<HistoricoCompra> historicoCompras)
        : base(id, numeroConta, dataCriacao)
    {
        ClienteId = cliente.Id;
        Cliente = cliente;
        Distribuicoes = distribuicoes;
        CustodiaFilhotes = custodiaFilhotes;
        HistoricoCompra = historicoCompras;
    }

    internal ContaGrafica(Cliente cliente)
        : base(0, cliente.Id)
    {
        ClienteId = cliente.Id;
        Cliente = cliente;
    }

    public static ContaGrafica Gerar(Cliente cliente)
        => new ContaGrafica(cliente);

    public void AdicionarDistribuicao(Distribuicao distribuicao)
        => Distribuicoes.Add(distribuicao);

    public void AdicionarCompra(HistoricoCompra compra)
        => HistoricoCompra.Add(compra);

    /// <summary>
    /// Realiza cálculo da carteira como total investido, valor atual carteira e PL total, a partir de uma cotação de fechamento.
    /// </summary>
    /// <param name="fechamento">Fechamento dos ativos da cesta recomendada</param>
    public ResumoCarteiraDto CalcularResumoDeRentabilidade(Cotacao fechamento)
    {
        if (!fechamento.ComposicaoCotacao.Any())
            throw new ApplicationException("Itens do fechamento inválido!");

        if (!CustodiaFilhotes.Any())
            throw new ApplicationException("Cliente não tem uma carteira populada.");

        decimal valorTotalInvestido = 0;
        decimal valorTotalAtualCarteira = 0;
        decimal plTotal = 0;

        foreach (var custodia in CustodiaFilhotes)
        {
            var precoFechamento = fechamento.ComposicaoCotacao.FirstOrDefault(x => x.Ticker == custodia.Ticker)?.PrecoFechamento ?? 0;

            var valorInvestido = custodia.CalcularValorInvestido();
            var valorAtual = custodia.CalcularValorAtualCarteira(precoFechamento);
            var plAtivo = custodia.CalcularPl(precoFechamento);

            valorTotalInvestido += valorInvestido;
            valorTotalAtualCarteira += valorAtual;
            plTotal += plAtivo;
        }

        return new ResumoCarteiraDto
        {
            ValorTotalInvestido = valorTotalInvestido,
            ValorAtualCarteira = valorTotalAtualCarteira,
            PlTotal = plTotal,
            RentabilidadePercentual = CalcularRentabilidade(valorTotalAtualCarteira, valorTotalInvestido)
        };
    }

    /// <summary>
    /// Obtem todo o histórico de aportes realizados.
    /// </summary>
    /// <returns>Lista de históico de aportes</returns>
    public List<HistoricoAporteDto> HistoricoAportes()
    {
        if (!HistoricoCompra.Any())
            throw new ApplicationException("Cliente ainda não tem compras realizadas.");

        var historicoCompraOrdenado = HistoricoCompra
            .DistinctBy(x => x.Data)
            .OrderBy(x => x.Data)
            .ToList();

        return historicoCompraOrdenado
            .GroupBy(x => new { x.Data.Year, x.Data.Month })
            .SelectMany(grupo =>
                grupo.OrderBy(x => x.Data)
                .Select((item, index) => new HistoricoAporteDto
                {
                    Data = item.Data,
                    Valor = item.ValorAporte,
                    Parcela = $"{index + 1}/{NUMERO_PARCELA_MAXIMO_COMPRA}"
                }))
            .OrderBy(x => x.Data)
            .ToList();
    }

    /// <summary>
    /// Obtem todo o histórico da evolução da carteira.
    /// </summary>
    /// <param name="fechamento">Fechamento dos ativos da cesta recomendada</param>
    /// <returns>Lista da evolução da carteira</returns>
    public List<EvolucaoCarteiraDto> CalcularEvolucaoCarteira(Cotacao fechamento)
    {
        if (!fechamento.ComposicaoCotacao.Any())
            throw new ApplicationException("Itens do fechamento inválido!");

        if (!HistoricoCompra.Any())
            throw new ApplicationException("Cliente ainda não tem compras realizadas.");

        var historicoCompraAgrupado = HistoricoCompra
            .OrderBy(x => x.Data)
            .GroupBy(x => new { x.Data, x.ValorAporte })
            .ToList();

        decimal valorAporteAnterior = 0;
        List<EvolucaoCarteiraDto> evolucaoCarteira = new() { };

        foreach (var grupo in historicoCompraAgrupado)
        {
            decimal valorCarteira = 0;
            valorAporteAnterior += grupo.Key.ValorAporte;

            foreach (var historico in grupo)
            {
                var precoFechamento = fechamento.ComposicaoCotacao.FirstOrDefault(x => x.Ticker == historico.Ticker)?.PrecoFechamento ?? 0;

                valorCarteira += historico.Quantidade * precoFechamento;
            }

            evolucaoCarteira.Add(new()
            {
                Data = grupo.Key.Data,
                ValorCarteira = valorCarteira,
                ValorInvestido = valorAporteAnterior,
                Rentabilidade = CalcularRentabilidade(valorCarteira, valorAporteAnterior)
            });
        }

        return evolucaoCarteira;
    }

    /// <summary>
    /// Calcula os detalhes de cada ativo (valor investido, atual, pl, pl percentual etc) com base na cotação atual
    /// </summary>
    /// <param name="fechamento">Fechamento dos ativos da cesta recomendada</param>
    /// <param name="valorTotalAtualCarteira">Valor total atual da carteira</param>
    /// <returns>Lista de detalhes de cada ativo da carteira</returns>
    public List<DetalheCarteiraDto> CalcularDetalhesCarteira(Cotacao fechamento, decimal valorTotalAtualCarteira)
    {
        if (!fechamento.ComposicaoCotacao.Any())
            throw new ApplicationException("Itens do fechamento inválido!");

        if (!CustodiaFilhotes.Any())
            throw new ApplicationException("Conta não tem uma carteira no momento.");

        var detalhesAtivos = CustodiaFilhotes
            .Select(custodia =>
            {
                var precoFechamento = fechamento.ComposicaoCotacao.FirstOrDefault(x => x.Ticker == custodia.Ticker)?.PrecoFechamento ?? 0;

                var valorInvestido = custodia.CalcularValorInvestido();
                var valorAtual = custodia.CalcularValorAtualCarteira(precoFechamento);
                var pl = custodia.CalcularPl(precoFechamento);
                var plPercentual = CalcularPlPercentual(valorAtual, valorInvestido);
                var composicaoCarteira = valorAtual < 1 ? 0 : (valorAtual / valorTotalAtualCarteira) * 100;

                return new DetalheCarteiraDto
                {
                    Ticker = custodia.Ticker,
                    Quantidade = custodia.Quantidade,
                    PrecoMedio = custodia.PrecoMedio,
                    CotacaoAtual = precoFechamento,
                    ValorAtual = valorAtual,
                    Pl = pl,
                    PlPercentual = plPercentual,
                    ComposicaoCarteira = Math.Round(composicaoCarteira, 2)
                };
            }).ToList();

        return detalhesAtivos;
    }

    protected override string GerarNumeroConta(int id)
        => $"FLH-{id:D6}";

    /// <summary>
    /// Calcula rentabilidade sobre um investimento
    /// </summary>
    /// <param name="valorAtualTotalCarteira">Valor total atual da carteira</param>
    /// <param name="valorTotalInvestido">Valor total investido</param>
    /// <returns>Valor da rentabilidade</returns>
    private decimal CalcularRentabilidade(decimal valorAtualTotalCarteira, decimal valorTotalInvestido)
    {
        if (valorAtualTotalCarteira < 1 || valorTotalInvestido < 1)
            return 0;

        var rentabilidade = ((valorAtualTotalCarteira / valorTotalInvestido) - 1) * 100;

        return Math.Round(rentabilidade, 2);
    }

    /// <summary>
    /// Calcula porcentagem de PL (Lucro/Prejuízo) por ativo
    /// </summary>
    /// <param name="valorAtual">Valor atual carteira</param>
    /// <param name="valorInvestido">valor investido na carteira</param>
    /// <returns>Valor em percentual de PL</returns>
    private decimal CalcularPlPercentual(decimal valorAtual, decimal valorInvestido)
    {
        if (valorAtual < 1)
            return 0;

        var plPercentual = ((valorAtual / valorInvestido) - 1) * 100;

        return Math.Round(plPercentual, 2);
    }
}