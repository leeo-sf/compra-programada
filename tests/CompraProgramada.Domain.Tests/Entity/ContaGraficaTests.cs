using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Tests.TestsUtils;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ContaGraficaTests
{
    [Fact]
    public async Task Gerar_DeveRetornarContaGraficaComSucesso_Quando_DadosValidosInformados()
    {
        var cliente = Cliente.Criar("Name", "11111111111", "email@test.com", 150);
        var conta = ContaGrafica.Gerar(cliente);

        conta.Id.Should().Be(0);
        conta.NumeroConta.Should().Be("FLH-000000");
        conta.DataCriacao.Should().NotBeAfter(DateTime.Now);
        conta.Tipo.Should().Be("FILHOTE");
        conta.CustodiaFilhotes.Should().BeEmpty();
        conta.Distribuicoes.Should().BeEmpty();
        conta.HistoricoCompra.Should().BeEmpty();
    }

    [Fact]
    public async Task AdicionarCompra_DeveAdicionarComSucesso_Quando_Solicitado()
    {
        var cliente = Cliente.Criar("Name", "11111111111", "email@test.com", 150);
        var conta = ContaGrafica.Gerar(cliente);

        conta.AdicionarCompra(HistoricoCompra.RegistrarHistorico(1, "PETR4", 10, 10, 10, 50, DateOnly.FromDateTime(DateTime.Now)));

        conta.HistoricoCompra.Should().NotBeNull();
        conta.HistoricoCompra.Count.Should().Be(1);
    }

    [Fact]
    public async Task AdicionarDistribuicao_DeveAdicionarComSucesso_Quando_Solicitado()
    {
        var cliente = Cliente.Criar("Name", "11111111111", "email@test.com", 150);
        var conta = ContaGrafica.Gerar(cliente);
        var ordemCompra = OrdemCompra.GerarOrdemCompra("PETR4", 100, 42);

        conta.AdicionarDistribuicao(Distribuicao.CriarDistribuicao(10, conta, ordemCompra));

        conta.Distribuicoes.Should().NotBeNull();
        conta.Distribuicoes.Count.Should().Be(1);
    }
}