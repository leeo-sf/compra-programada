using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Enum;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class OrdemCompraDetalheTests
{
    [Theory]
    [InlineData(OrdemCompraTipo.Padrao, "PETR4", 1)]
    [InlineData(OrdemCompraTipo.Fracionario, "PETR4", 1)]
    public async Task GerarDetalhe_DeveRetornarContaMasterComSucesso_Quando_DadosValidosInformados(OrdemCompraTipo tipo, string ticker, int quantidade)
    {
        var detalhe = OrdemCompraDetalhe.GerarDetalhe(tipo, ticker, quantidade);

        detalhe.Id.Should().Be(0);
        detalhe.Tipo.Should().Be(tipo);
        detalhe.Ticker.Should().Be(ticker);
        detalhe.Quantidade.Should().Be(quantidade);
        detalhe.OrdemCompraId.Should().Be(0);
        detalhe.OrdemCompra.Should().BeNull();
    }
}