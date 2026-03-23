using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ContaTests
{
    [Fact]
    public async Task GerarContaGrafica_DeveRetornarContaGraficaComSucesso_Quando_DadosValidosInformados()
    {
        List<CustodiaFilhote> custodias = new() { CustodiaFilhote.GerarCustodia("PETR4") };
        var conta = ContaGrafica.Gerar(1, custodias);

        conta.Id.Should().Be(0);
        conta.NumeroConta.Should().Be("FLH-000001");
        conta.DataCriacao.Should().NotBeAfter(DateTime.Now);
        conta.Tipo.Should().Be("FILHOTE");
        conta.CustodiaFilhotes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GerarContaMaster_DeveRetornarContaMasterComSucesso_Quando_DadosValidosInformados()
    {
        List<CustodiaMaster> custodias = new() { CustodiaMaster.CriarCustodia(1, "PETR4") };
        var conta = ContaMaster.Gerar(1, custodias);

        conta.Id.Should().Be(0);
        conta.NumeroConta.Should().Be("MST-000001");
        conta.DataCriacao.Should().NotBeAfter(DateTime.Now);
        conta.Tipo.Should().Be("MASTER");
        conta.CustodiaMasters.Should().NotBeEmpty();
    }
}