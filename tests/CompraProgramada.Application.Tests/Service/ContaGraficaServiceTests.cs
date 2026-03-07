using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using CompraProgramada.Application.Interface;
using FluentAssertions;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class ContaGraficaServiceTests
{
    private readonly Mock<ICestaRecomendadaService> _cestaServiceMock;
    private readonly Mock<IContaGraficaRepository> _contaRepoMock;
    private readonly ContaGraficaService _service;

    public ContaGraficaServiceTests()
    {
        _cestaServiceMock = new Mock<ICestaRecomendadaService>();
        _contaRepoMock = new Mock<IContaGraficaRepository>();

        _service = new ContaGraficaService(_cestaServiceMock.Object, _contaRepoMock.Object);
    }

    [Fact]
    public async Task GerarContaGraficaAsync_DeveCriarContaComCustodias()
    {
        var cesta = new CestaRecomandadaDto { Id = 1, Nome = "C1", DataCriacao = DateTime.UtcNow, Itens = new List<ComposicaoCestaRecomendadaDto> { new ComposicaoCestaRecomendadaDto { Ticker = "PETR4", Percentual = 50, Id = 1, CestaId = 1 } } };
        _cestaServiceMock.Setup(c => c.ObterCestaAtivaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(cesta));

        _contaRepoMock.Setup(r => r.CriarAsync(It.IsAny<ContaGrafica>(), It.IsAny<CancellationToken>())).ReturnsAsync((ContaGrafica c, CancellationToken ct) => c with { Id = 10 });

        var result = await _service.GerarContaGraficaAsync(5, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(5);
        result.Value.CustodiaFilhotes.Should().HaveCount(1);
    }

    [Fact]
    public async Task RegistrarComprasAsync_DeveRetornarErro_QuandoListaVazia()
    {
        var result = await _service.RegistrarComprasAsync(new List<HistoricoCompraDto>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task RegistrarComprasAsync_DeveChamarRepositorio_QuandoExistirCompras()
    {
        var compras = new List<HistoricoCompraDto> { new HistoricoCompraDto(0, 100m, DateOnly.FromDateTime(DateTime.UtcNow), 1) };

        _contaRepoMock.Setup(r => r.RegistrarHistoricoCompraAysnc(It.IsAny<List<HistoricoCompra>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.RegistrarComprasAsync(compras, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _contaRepoMock.Verify(r => r.RegistrarHistoricoCompraAysnc(It.IsAny<List<HistoricoCompra>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}