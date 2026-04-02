using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class ImpostoRendaServiceTests
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ImpostoRendaService _sut;

    public ImpostoRendaServiceTests()
    {
        _kafkaProducer = Substitute.For<IKafkaProducer>();
        _sut = new(_kafkaProducer);
    }

    [Fact]
    public async Task ImpostoRenda_Deve_PublicarMensagem_Quando_Solicitado()
    {
        // Arrange
        var clientes = FakerRequest.ClientesAtivos().Generate();
        var contaA = clientes.Select(x => x.ContaGrafica).FirstOrDefault(x => x.Id == 1)!;
        var contaB = clientes.Select(x => x.ContaGrafica).FirstOrDefault(x => x.Id == 2)!;
        var contaC = clientes.Select(x => x.ContaGrafica).FirstOrDefault(x => x.Id == 3)!;

        List<Distribuicao> distribuicoes = new()
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
        };

        var quantidadeItensPublicados = 5;

        // Act
        var result = await _sut.PublicarIR(distribuicoes, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().Be(quantidadeItensPublicados);
    }

    [Theory]
    [InlineData(280, 0.01)]
    [InlineData(974, 0.04)]
    [InlineData(49561, 2.47)]
    [InlineData(2, 0.00)]
    public void ImpostoRenda_Deve_CalcularIr_Quando_Solicitado(decimal valorOperacao, decimal resultadoEsperado)
    {
        // Arrange & Act
        var result = _sut.CalcularImpostoDeRenda(valorOperacao);

        // Assert
        result.Should().Be(resultadoEsperado);
    }
}