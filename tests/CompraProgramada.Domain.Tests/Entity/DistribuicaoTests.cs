using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using FluentAssertions;
using System.Net;

namespace CompraProgramada.Domain.Tests.Entity;

public class DistribuicaoTests
{
    [Fact]
    public async Task CriarDistribuicao_DeveRetornarDistribuicaoComSucesso_Quando_DadosValidosInformados()
    {
        var distribuicao = Distribuicao.CriarDistribuicao(1, 1, "PETR4", 10, 47.98m);

        distribuicao.Id.Should().Be(0);
        distribuicao.OrdemCompraId.Should().Be(1);
        distribuicao.ContaGraficaId.Should().Be(1);
        distribuicao.Ticker.Should().Be("PETR4");
        distribuicao.QuantidadeAlocada.Should().Be(10);
        distribuicao.ValorOperacao.Should().Be(47.98m);
    }

    [Fact]
    public async Task CriarDistribuicao_DeveRetornarTickerNaoPreenchidoException_Quando_EnviarTickerVazio()
    {
        var act = () => Distribuicao.CriarDistribuicao(1, 1, "", 10, 47.98m);
        var exception = act.Should().Throw<TickerNaoPreenchidoException>().Which;

        exception.Message.Should().Be("O nome do ativo não pode estar em branco.");
        exception.Codigo.Should().Be("TICKER_INVALIDO");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CriarDistribuicao_DeveRetornarQuantidadeNegativaException_Quando_EnviarQuantidadeNegativa()
    {
        var act = () => Distribuicao.CriarDistribuicao(1, 1, "PETR4", -1, 47.98m);
        var exception = act.Should().Throw<QuantidadeNegativaException>().Which;

        exception.Message.Should().Be("Quantidade não pode ser negativa.");
        exception.Codigo.Should().Be("QUANTIDADE_NEGATIVA");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}