using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using FluentAssertions;
using System.Net;

namespace CompraProgramada.Domain.Tests.Entity;

public class CustodiaFilhoteTests
{
    [Fact]
    public async Task GerarCustodia_DeveRetornarCustodiaFilhoteComSucesso_Quando_DadosValidosInformados()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");

        custodia.Id.Should().Be(0);
        custodia.ContaGraficaId.Should().Be(0);
        custodia.Ticker.Should().Be("PETR4");
        custodia.PrecoMedio.Should().Be(0);
        custodia.Quantidade.Should().Be(0);
    }

    [Fact]
    public async Task GerarCustodia_DeveRetornarTickerNaoPreenchidoException_Quando_EnviarTickerVazio()
    {
        var act = () => CustodiaFilhote.GerarCustodia("");
        var exception = act.Should().Throw<TickerNaoPreenchidoException>().Which;

        exception.Message.Should().Be("O nome do ativo não pode estar em branco.");
        exception.Codigo.Should().Be("TICKER_INVALIDO");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Atuailzar_DeveRetornarCustodiaFilhoteAtualizadaComSucesso_Quando_DadosValidosInformados()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");
        custodia.Atualizar(42, 10);

        custodia.Id.Should().Be(0);
        custodia.ContaGraficaId.Should().Be(0);
        custodia.Ticker.Should().Be("PETR4");
        custodia.PrecoMedio.Should().Be(42);
        custodia.Quantidade.Should().Be(10);
    }

    [Fact]
    public async Task Atuaizar_DeveRetornarQuantidadeNegativaException_Quando_EnviarQuantidadeNegativa()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");
        custodia.Atualizar(42, 10);
        var act = () => custodia.Atualizar(42, -1);
        var exception = act.Should().Throw<QuantidadeNegativaException>().Which;

        exception.Message.Should().Be("Quantidade não pode ser negativa.");
        exception.Codigo.Should().Be("QUANTIDADE_NEGATIVA");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}