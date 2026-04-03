using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Exceptions;
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
        custodia.AdicionarNovaQuantidade(10);

        custodia.Id.Should().Be(0);
        custodia.ContaGraficaId.Should().Be(0);
        custodia.Ticker.Should().Be("PETR4");
        custodia.PrecoMedio.Should().Be(0);
        custodia.Quantidade.Should().Be(10);
    }

    [Fact]
    public async Task Atuailzar_DeveRetornarCustodiaFilhoteAtualizadaComSucesso_Quando_NovasQuantidadesInformadas()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");
        custodia.AdicionarNovaQuantidade(8);
        custodia.AdicionarNovaQuantidade(4);

        custodia.Id.Should().Be(0);
        custodia.ContaGraficaId.Should().Be(0);
        custodia.Ticker.Should().Be("PETR4");
        custodia.PrecoMedio.Should().Be(0);
        custodia.Quantidade.Should().Be(12);
    }

    [Fact]
    public async Task Atuaizar_DeveRetornarQuantidadeNegativaException_Quando_EnviarQuantidadeNegativa()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");
        var act = () => custodia.AdicionarNovaQuantidade(-1);
        var exception = act.Should().Throw<QuantidadeNegativaException>().Which;

        exception.Message.Should().Be("Quantidade não pode ser negativa.");
        exception.Codigo.Should().Be("QUANTIDADE_NEGATIVA");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CalcularPrecoMedio_DeveRetornarZero_Quando_QuantidadeNovaInferior_A_1()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");

        var result = custodia.CalcularPrecoMedio(10, 0);

        result.Should().Be(0);
    }

    [Fact]
    public async Task CalcularPrecoMedio_DeveRetornarPM_Quando_Quantidade_Maior_Que_Zero()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");
        custodia.AdicionarNovaQuantidade(10);

        var result = custodia.CalcularPrecoMedio(35, 2);

        result.Should().Be(9);
    }

    [Fact]
    public async Task CalcularPL_DeveRetornarQuantidadeCustodiaException_Quando_QuantidadeInferior_A_Zero()
    {
        var custodia = new CustodiaFilhote(1, 1, "PETR4", 10, -1);

        var act = () => custodia.CalcularPl(35);
        var exception = act.Should().Throw<QuantidadeCustodiaException>().Which;

        exception.Message.Should().Be("Quantidade deve ser maior que zero.");
        exception.Codigo.Should().Be("QUANTIDADE_INVALIDA");
        exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CalcularPl_DeveRetornarPl_Quando_Quantidade_Maior_Que_1()
    {
        var custodia = CustodiaFilhote.GerarCustodia("PETR4");
        custodia.AdicionarNovaQuantidade(24);

        var result = custodia.CalcularPl(37);

        result.Should().Be(888);
    }
}