using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class HistoricoCompraTests
{
    [Theory]
    [InlineData("PETR4", 10, 10, 10, 10)]
    [InlineData("AAPL4", 10, 26, 30, 10)]
    public async Task RegistrarHistorico_DeveRetornarHistoricoComSucesso_Quando_DadosValidosInformados(string ticker, int quantidade, decimal precoExecutado, decimal precoMedio, decimal valorAporte)
    {
        var historico = HistoricoCompra.RegistrarHistorico(1, ticker, quantidade, precoExecutado, precoMedio, valorAporte, DateOnly.FromDateTime(DateTime.Now));

        historico.Id.Should().Be(0);
        historico.ContaGraficaId.Should().Be(1);
        historico.Ticker.Should().Be(ticker);
        historico.Quantidade.Should().Be(quantidade);
        historico.PrecoExecutado.Should().Be(precoExecutado);
        historico.PrecoMedio.Should().Be(precoMedio);
        historico.ValorAporte.Should().Be(valorAporte);
        historico.Data.Should().Be(DateOnly.FromDateTime(DateTime.Now));
        historico.ContaGrafica.Should().BeNull();
    }

    [Fact]
    public async Task RegistrarHistorico_DeveLancarException_Quando_Ticker_NaoInformado()
    {
        var act = () => HistoricoCompra.RegistrarHistorico(1, "", 10, 10, 10, 10, DateOnly.FromDateTime(DateTime.Now));
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("O ativo precisa ser definido!");
    }

    [Fact]
    public async Task RegistrarHistorico_DeveLancarException_Quando_Quantidade_Negativa()
    {
        var act = () => HistoricoCompra.RegistrarHistorico(1, "PETR4", -10, 10, 10, 10, DateOnly.FromDateTime(DateTime.Now));
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Quantidade não pode ser negativa");
    }

    [Fact]
    public async Task RegistrarHistorico_DeveLancarException_Quando_PrecoExecutado_Negativo()
    {
        var act = () => HistoricoCompra.RegistrarHistorico(1, "PETR4", 10, -10, 10, 10, DateOnly.FromDateTime(DateTime.Now));
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Preco de fechamento não pode ser negativo");
    }
}