using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Exceptions;
using FluentAssertions;
using System.Net;

namespace CompraProgramada.Domain.Tests.Entity;

public class DistribuicaoTests
{
    [Fact]
    public async Task CriarDistribuicao_DeveRetornarDistribuicaoComSucesso_Quando_DadosValidosInformados()
    {
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@test.com", 150));
        var conta = ContaGrafica.Gerar(cliente);
        var ordemCompra = OrdemCompra.GerarOrdemCompra("PETR4", 100, 47.98m);

        var distribuicao = Distribuicao.CriarDistribuicao(10, conta, ordemCompra);

        distribuicao.Id.Should().Be(0);
        distribuicao.OrdemCompraId.Should().Be(0);
        distribuicao.ContaGraficaId.Should().Be(0);
        distribuicao.Ticker.Should().Be("PETR4");
        distribuicao.QuantidadeAlocada.Should().Be(10);
        distribuicao.ValorOperacao.Should().Be(479.80m);
    }

    [Fact]
    public async Task CriarDistribuicao_DeveRetornarTickerNaoPreenchidoException_Quando_EnviarTickerVazio()
    {
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@test.com", 150));
        var conta = ContaGrafica.Gerar(cliente);
        var ordemCompra = OrdemCompra.GerarOrdemCompra("", 100, 42);
        ;
        var act = () => Distribuicao.CriarDistribuicao(10, conta, ordemCompra);
        var exception = act.Should().Throw<TickerNaoPreenchidoException>().Which;

        exception.Message.Should().Be("O nome do ativo não pode estar em branco.");
        exception.Codigo.Should().Be("TICKER_INVALIDO");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CriarDistribuicao_DeveRetornarQuantidadeNegativaException_Quando_EnviarQuantidadeNegativa()
    {
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@test.com", 150));
        var conta = ContaGrafica.Gerar(cliente);
        var ordemCompra = OrdemCompra.GerarOrdemCompra("PETR4", 100, 42);

        var act = () => Distribuicao.CriarDistribuicao(-1, conta, ordemCompra);
        var exception = act.Should().Throw<QuantidadeNegativaException>().Which;

        exception.Message.Should().Be("Quantidade não pode ser negativa.");
        exception.Codigo.Should().Be("QUANTIDADE_NEGATIVA");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}