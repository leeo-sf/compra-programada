using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ComposicaoCotacaoTests
{
    [Theory]
    [InlineData("PETR4", 43.79)]
    [InlineData("AAPL4", 89.30)]
    [InlineData("ITUB4", 39.87)]
    public async Task CriarItemNaCesta_DeveRetornarComposicaoCestaComSucesso_Quando_Solicitado(string ticker, decimal precoFechamento)
    {
        var composicaoCesta = ComposicaoCotacao.CriarItem(ticker, precoFechamento);

        composicaoCesta.Id.Should().Be(0);
        composicaoCesta.CotacaoId.Should().Be(0);
        composicaoCesta.Ticker.Should().Be(ticker);
        composicaoCesta.PrecoFechamento.Should().Be(precoFechamento);
    }
}