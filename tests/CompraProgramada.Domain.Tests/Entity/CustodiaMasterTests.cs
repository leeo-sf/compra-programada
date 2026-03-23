using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class CustodiaMasterTests
{
    [Fact]
    public async Task CriarCustodia_DeveRetornarCustodiaMasterComSucesso_Quando_DadosValidosInformados()
    {
        var custodia = CustodiaMaster.CriarCustodia(1, "PETR4");

        custodia.Id.Should().Be(0);
        custodia.ContaMasterId.Should().Be(1);
        custodia.Ticker.Should().Be("PETR4");
        custodia.QuantidadeResiduo.Should().Be(0);
    }

    [Theory]
    [InlineData(150, 142, 8)]
    [InlineData(2, 1, 1)]
    [InlineData(57, 48, 9)]
    [InlineData(48, 48, 0)]
    [InlineData(45, 42, 3)]
    public async Task AtualizarResiduo_DeveRetornar_NovaQuantidadeResiduo_Quando_Atualizar(int qtdNovosComprados, int qtdUtilizada, int qtdResiduoEsperada)
    {
        var custodia = CustodiaMaster.CriarCustodia(1, "PETR4");
        custodia.AtualizarResiduo(qtdNovosComprados, qtdUtilizada);

        custodia.QuantidadeResiduo.Should().Be(qtdResiduoEsperada);
    }

    [Theory]
    [InlineData(100, 98, 80, 82, 0)]
    [InlineData(27, 26, 25, 23, 3)]
    [InlineData(48, 48, 47, 40, 7)]
    public async Task AtualizarResiduo_DeveRetornar_NovaQuantidadeResiduo_Quando_JaTiverQuantidade(int qtdCompradoPrimeiraVez, int qtdUtilizadaPrimeiraVez, int qtdNovosComprados, int qtdUtilizada, int qtdResiduoEsperada)
    {
        var custodia = CustodiaMaster.CriarCustodia(1, "PETR4");
        custodia.AtualizarResiduo(qtdCompradoPrimeiraVez, qtdUtilizadaPrimeiraVez);
        custodia.AtualizarResiduo(qtdNovosComprados, qtdUtilizada);

        custodia.QuantidadeResiduo.Should().Be(qtdResiduoEsperada);
    }

    [Theory]
    [InlineData(100, 100, 128, 128)]
    [InlineData(69, 68, 1, 0)]
    [InlineData(82, 80, 40, 38)]
    [InlineData(38, 30, 5, 3)]
    public async Task CalcularNecessidadeLiquidaCompra_DeveRetornar_QtdCompra_Quando_Solicitado(int qtdNovosComprados, int qtdUtilizada, int qtdDesejadaCompra, int qtdCompraEsperada)
    {
        var custodia = CustodiaMaster.CriarCustodia(1, "PETR4");
        custodia.AtualizarResiduo(qtdNovosComprados, qtdUtilizada);

        var qtdRetornada = custodia.CalculaNecessidadeLiquidaCompra(qtdDesejadaCompra);

        qtdRetornada.Should().Be(qtdCompraEsperada);
    }
}