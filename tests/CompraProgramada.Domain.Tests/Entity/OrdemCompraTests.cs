using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.Tests.TestsUtils;
using FluentAssertions;
using System.Net;

namespace CompraProgramada.Domain.Tests.Entity;

public class OrdemCompraTests
{
    [Theory]
    [InlineData("PETR4", 80, 42.83)]
    [InlineData("VALE3", 98, 62)]
    [InlineData("ITUB4", 77, 30)]
    [InlineData("BBDC4", 12, 15)]
    [InlineData("WEGE3", 26, 40)]
    public void GerarOrdemCompra_DeveRetornarOrdemCompra_ComLoteFracionario_Quando_QuantidadeCompra_InferiorA100(string ticker, int quantidadeTotal, decimal precoUnitario)
    {
        var valorTotal = quantidadeTotal * precoUnitario;
        var ordemCompra = OrdemCompra.GerarOrdemCompra(ticker, quantidadeTotal, precoUnitario, valorTotal);

        ordemCompra.Id.Should().Be(0);
        ordemCompra.Ticker.Should().Be(ticker);
        ordemCompra.QuantidadeTotal.Should().Be(quantidadeTotal);
        ordemCompra.PrecoUnitario.Should().Be(precoUnitario);
        ordemCompra.ValorTotal.Should().Be(valorTotal);
        ordemCompra.Detalhes.Count.Should().Be(1);
        ordemCompra.Detalhes.First().Id.Should().Be(0);
        ordemCompra.Detalhes.First().Tipo.Should().Be("FRACIONARIO");
        ordemCompra.Detalhes.First().Ticker.Should().Be($"{ticker}F");
        ordemCompra.Detalhes.First().Quantidade.Should().Be(quantidadeTotal);
        ordemCompra.Detalhes.First().OrdemCompraId.Should().Be(0);
    }

    [Theory]
    [InlineData("PETR4", 100, 42.83)]
    [InlineData("VALE3", 800, 62)]
    [InlineData("ITUB4", 300, 30)]
    [InlineData("BBDC4", 10000, 15)]
    [InlineData("WEGE3", 600, 40)]
    public void GerarOrdemCompra_DeveRetornarOrdemCompra_LotePadrao_Quando_QuantidadeCompra_IgualA100(string ticker, int quantidadeTotal, decimal precoUnitario)
    {
        var ordemCompra = OrdemCompra.GerarOrdemCompra(ticker, quantidadeTotal, precoUnitario, quantidadeTotal * precoUnitario);

        ordemCompra.Detalhes.Count.Should().Be(1);
        ordemCompra.Detalhes.First().Id.Should().Be(0);
        ordemCompra.Detalhes.First().Tipo.Should().Be("PADRAO");
        ordemCompra.Detalhes.First().Ticker.Should().Be(ticker);
        ordemCompra.Detalhes.First().Quantidade.Should().Be(quantidadeTotal);
        ordemCompra.Detalhes.First().OrdemCompraId.Should().Be(0);
    }

    [Theory]
    [InlineData("PETR4", 175, 42.83, 100, 75)]
    [InlineData("VALE3", 810, 62, 800, 10)]
    [InlineData("ITUB4", 349, 30, 300, 49)]
    [InlineData("BBDC4", 1091, 15, 1000, 91)]
    [InlineData("WEGE3", 601, 40, 600, 1)]
    [InlineData("AAPL4", 20044, 72.90, 20000, 44)]
    public void GerarOrdemCompra_DeveRetornarOrdemCompra_LotePadrao_E_Fracionario_Quando_QuantidadeCompra_SuperiorA100(string ticker, int quantidadeTotal, decimal precoUnitario, int qtdLotePadrao, int qtdLoteFracionario)
    {
        var ordemCompra = OrdemCompra.GerarOrdemCompra(ticker, quantidadeTotal, precoUnitario, quantidadeTotal * precoUnitario);

        var lotePadrao = ordemCompra.Detalhes.FirstOrDefault(x => x.Tipo == "PADRAO");
        var loteFracionario = ordemCompra.Detalhes.FirstOrDefault(x => x.Tipo == "FRACIONARIO");

        ordemCompra.Detalhes.Count.Should().Be(2);

        lotePadrao?.Id.Should().Be(0);
        lotePadrao?.Tipo.Should().Be("PADRAO");
        lotePadrao?.Ticker.Should().Be(ticker);
        lotePadrao?.Quantidade.Should().Be(qtdLotePadrao);
        lotePadrao?.OrdemCompraId.Should().Be(0);

        loteFracionario?.Id.Should().Be(0);
        loteFracionario?.Tipo.Should().Be("FRACIONARIO");
        loteFracionario?.Ticker.Should().Be($"{ticker}F");
        loteFracionario?.Quantidade.Should().Be(qtdLoteFracionario);
        loteFracionario?.OrdemCompraId.Should().Be(0);
    }
}