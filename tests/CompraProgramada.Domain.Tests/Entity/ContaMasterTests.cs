using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ContaMasterTests
{
    [Fact]
    public async Task Gerar_DeveRetornarContaMasterComSucesso_Quando_DadosValidosInformados()
    {
        List<CustodiaMaster> custodias = new() { CustodiaMaster.CriarCustodia(1, "PETR4") };
        var conta = ContaMaster.Gerar(1, custodias);

        conta.Id.Should().Be(0);
        conta.NumeroConta.Should().Be("MST-000001");
        conta.DataCriacao.Should().NotBeAfter(DateTime.Now);
        conta.Tipo.Should().Be("MASTER");
        conta.CustodiaMasters.Count.Should().Be(1);
    }
}