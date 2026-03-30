using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class ContaGraficaServiceTests
{
    private readonly IContaGraficaRepository _contaGraficaRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ContaGraficaService _sut;

    public ContaGraficaServiceTests()
    {
        _contaGraficaRepository = Substitute.For<IContaGraficaRepository>();
        _cotacaoService = Substitute.For<ICotacaoService>();
        _sut = new(_contaGraficaRepository, _cotacaoService);
    }

    [Fact]
    public async Task ContaGrafica_Deve_CriarConta_Quando_NaoHouverErro()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _contaGraficaRepository.CriarAsync(Arg.Any<ContaGrafica>(), Arg.Any<CancellationToken>())
            .Returns(ContaGrafica.Gerar(cliente));

        // Act
        var result = await _sut.GerarContaGraficaAsync(cliente, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Cliente.Should().NotBeNull();
    }

    [Fact]
    public async Task ContaGrafica_Deve_AtualizarContas_Quando_NaoHouverErro()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var contas = new List<ContaGrafica> { ContaGrafica.Gerar(cliente) };

        _contaGraficaRepository.AtualizarContasAsync(Arg.Any<List<ContaGrafica>>(), Arg.Any<CancellationToken>())
            .Returns(contas);

        // Act
        var result = await _sut.AtualizarContasAsync(contas, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ContaGrafica_Deve_Falhar_Ao_AtualizarContas_Quando_ContasVaziaEnviadas()
    {
        // Arrange & Act
        var result = await _sut.AtualizarContasAsync(new() { }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Message.Should().Be("Nenhuma conta gráfica informada para atualização.");
        result.Value.Should().BeNull();
    }
}