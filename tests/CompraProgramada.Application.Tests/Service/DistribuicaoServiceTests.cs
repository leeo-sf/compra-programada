using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class DistribuicaoServiceTests
{
    private readonly ILogger<DistribuicaoService> _logger;
    private readonly ICustodiaMasterService _custodiaMasterService;
    private readonly IContaGraficaService _contaGraficaService;
    private readonly DistribuicaoService _sut;

    public DistribuicaoServiceTests()
    {
        _logger = Substitute.For<ILogger<DistribuicaoService>>();
        _custodiaMasterService = Substitute.For<ICustodiaMasterService>();
        _contaGraficaService = Substitute.For<IContaGraficaService>();
        _sut = new(_logger, _custodiaMasterService, _contaGraficaService);
    }

    [Fact]
    public async Task DistribuicaoService_Deve_DistribuirParaCustodias_Quando_NaoOcorrerErro()
    {
        // Arrange
        var clientes = FakerRequest.ClientesAtivos().Generate();
        var ordensCompra = FakerRequest.OrdensCompraEmitidas();

        _custodiaMasterService.ObterResiduosNaoDistribuidos(Arg.Any<CancellationToken>())
            .Returns(new List<CustodiaMaster>());

        _contaGraficaService.AtualizarContasAsync(Arg.Any<List<ContaGrafica>>(), Arg.Any<CancellationToken>())
            .Returns(new List<ContaGrafica>());

        // Act
        var result = await _sut.DistribuirParaCustodiasAsync(clientes, ordensCompra, DateTime.Now, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(new List<ContaGrafica>());
    }

    [Theory]
    [MemberData(nameof(CalcularDistribuicao))]
    public void DistribuicaoService_Deve_CalcularDistribuicao_Quando_Solicitado(List<Cliente> clientesAtivos, List<OrdemCompra> ordensCompra, List<CustodiaMaster> residuosAtuais, List<Distribuicao> distribuicoes)
    {
        // Arrange & Act
        var result = _sut.CalcularDistribuicao(clientesAtivos, ordensCompra, residuosAtuais, DateTime.Now);
        var distribuicoesRealizadas = result.SelectMany(x => x.Distribuicoes).ToList();

        // Assert
        result.Should().NotBeNullOrEmpty();
        distribuicoesRealizadas.Should().BeEquivalentTo(distribuicoes, opt =>
            opt.Using<DateTime>(ctx =>
                ctx.Subject.Date.Should().Be(ctx.Expectation.Date))
            .WhenTypeIs<DateTime>());
    }

    public static TheoryData<List<Cliente>, List<OrdemCompra>, List<CustodiaMaster>, List<Distribuicao>> CalcularDistribuicao()
    {
        var clientes = FakerRequest.ClientesAtivos().Generate();
        var contaA = clientes.Select(x => x.ContaGrafica).FirstOrDefault(x => x.Id == 1)!;
        var contaB = clientes.Select(x => x.ContaGrafica).FirstOrDefault(x => x.Id == 2)!;
        var contaC = clientes.Select(x => x.ContaGrafica).FirstOrDefault(x => x.Id == 3)!;

        return new()
        {
            {
                clientes,
                new()
                {
                    OrdemCompra.GerarOrdemCompra("PETR4", 30, 35),
                    OrdemCompra.GerarOrdemCompra("VALE3", 14, 62),
                    OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30),
                    OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15),
                    OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)
                },
                new() { },
                new()
                {
                    Distribuicao.CriarDistribuicao(8, contaA, OrdemCompra.GerarOrdemCompra("PETR4", 30, 35)),
                    Distribuicao.CriarDistribuicao(3, contaA, OrdemCompra.GerarOrdemCompra("VALE3", 14, 62)),
                    Distribuicao.CriarDistribuicao(6, contaA, OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30)),
                    Distribuicao.CriarDistribuicao(10, contaA, OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)),
                    Distribuicao.CriarDistribuicao(2, contaA, OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)),
                    Distribuicao.CriarDistribuicao(17, contaB, OrdemCompra.GerarOrdemCompra("PETR4", 30, 35)),
                    Distribuicao.CriarDistribuicao(8, contaB, OrdemCompra.GerarOrdemCompra("VALE3", 14, 62)),
                    Distribuicao.CriarDistribuicao(13, contaB, OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30)),
                    Distribuicao.CriarDistribuicao(19, contaB, OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)),
                    Distribuicao.CriarDistribuicao(4, contaB, OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)),
                    Distribuicao.CriarDistribuicao(4, contaC, OrdemCompra.GerarOrdemCompra("PETR4", 30, 35)),
                    Distribuicao.CriarDistribuicao(2, contaC, OrdemCompra.GerarOrdemCompra("VALE3", 14, 62)),
                    Distribuicao.CriarDistribuicao(3, contaC, OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30)),
                    Distribuicao.CriarDistribuicao(5, contaC, OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)),
                    Distribuicao.CriarDistribuicao(1, contaC, OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40))
                }
            }
        };
    }
}