using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ComposicaoCestaTests
{
    [Theory]
    [InlineData("PETR4", 30)]
    [InlineData("AAPL4", 15.80)]
    [InlineData("ITUB4", 25)]
    public async Task CriarItemNaCesta_DeveRetornarComposicaoCestaComSucesso_Quando_Solicitado(string ticker, decimal percentual)
    {
        var composicaoCesta = ComposicaoCesta.CriaItemNaCesta(ticker, percentual);

        composicaoCesta.Id.Should().Be(0);
        composicaoCesta.CestaId.Should().Be(0);
        composicaoCesta.Ticker.Should().Be(ticker);
        composicaoCesta.Percentual.Should().Be(percentual);
    }
}