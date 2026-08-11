using CompraProgramada.Shared.Enum;
using CompraProgramada.Shared.Exceptions;
using System.Net;

namespace CompraProgramada.Domain.Entity;

public class OrdemCompra
{
    public int Id { get; }
    public string Ticker { get; }
    public int QuantidadeTotal { get; }
    public decimal PrecoUnitario { get; }
    public decimal ValorTotal { get; }
    public DateTime Data { get; }
    public List<Distribuicao> Distribuicoes { get; private set; } = new List<Distribuicao>();
    public List<OrdemCompraDetalhe> Detalhes { get; private set; } = new List<OrdemCompraDetalhe>();

    private OrdemCompra() { }

    internal OrdemCompra(int id, string ticker, int quantidadeTotal, decimal precoUnitario, decimal valorTotal, DateTime data, List<OrdemCompraDetalhe> detalhes)
    {
        Id = id;
        Ticker = ticker;
        QuantidadeTotal = quantidadeTotal;
        PrecoUnitario = precoUnitario;
        ValorTotal = valorTotal;
        Data = data;
        Detalhes = detalhes;
    }

    public static OrdemCompra GerarOrdemCompra(string ticker, int quantidadeTotal, decimal precoUnitario)
    {
        if (quantidadeTotal < 1)
            throw new AppException("Solicitação de registro de ordem de compra inferior a 1", "ORDEM_COMPRA_INVALIDA", HttpStatusCode.BadRequest);

        List<OrdemCompraDetalhe> detalhes = [];
        var multiplosPresente = Math.DivRem(quantidadeTotal, 100, out int restos);

        if (restos > 0)
            detalhes.Add(OrdemCompraDetalhe.GerarDetalhe(OrdemCompraTipo.Fracionario, $"{ticker}F", restos));

        if (multiplosPresente > 0)
            detalhes.Add(OrdemCompraDetalhe.GerarDetalhe(OrdemCompraTipo.Padrao, ticker, multiplosPresente * 100));

        var valorOrdemCompra = quantidadeTotal * precoUnitario;

        return new(0, ticker.ToUpper(), quantidadeTotal, precoUnitario, valorOrdemCompra, DateTime.UtcNow, detalhes);
    }
}