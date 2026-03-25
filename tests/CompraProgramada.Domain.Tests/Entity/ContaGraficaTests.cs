using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ContaGraficaTests
{
    [Fact]
    public async Task Gerar_DeveRetornarContaGraficaComSucesso_Quando_DadosValidosInformados()
    {
        List<CustodiaFilhote> custodias = new() { CustodiaFilhote.GerarCustodia("PETR4") };
        var conta = ContaGrafica.Gerar(1);

        conta.Id.Should().Be(0);
        conta.NumeroConta.Should().Be("FLH-000001");
        conta.DataCriacao.Should().NotBeAfter(DateTime.Now);
        conta.Tipo.Should().Be("FILHOTE");
        conta.CustodiaFilhotes.Should().BeEmpty();
        conta.Distribuicoes.Should().BeEmpty();
        conta.HistoricoCompra.Should().BeEmpty();
    }

    [Fact]
    public async Task AdicionarCompra_DeveAdicionarComSucesso_Quando_Solicitado()
    {
        List<CustodiaFilhote> custodias = new() { CustodiaFilhote.GerarCustodia("PETR4") };
        var conta = ContaGrafica.Gerar(1);

        conta.AdicionarCompra(HistoricoCompra.RegistrarHistorico(1, "PETR4", 10, 10, 10, 50, DateOnly.FromDateTime(DateTime.Now)));

        conta.HistoricoCompra.Should().NotBeNull();
        conta.HistoricoCompra.Count.Should().Be(1);
    }

    [Fact]
    public async Task AdicionarDistribuicao_DeveAdicionarComSucesso_Quando_Solicitado()
    {
        List<CustodiaFilhote> custodias = new() { CustodiaFilhote.GerarCustodia("PETR4") };
        var conta = ContaGrafica.Gerar(1);

        conta.AdicionarDistribuicao(Distribuicao.CriarDistribuicao(1, 1, "PETR4", 10, 10));

        conta.Distribuicoes.Should().NotBeNull();
        conta.Distribuicoes.Count.Should().Be(1);
    }
}