using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Application.Handler;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Handler;

public class CriarCestaHandlerTests
{

    private readonly Mock<ILogger<CriarCestaHandler>> _loggerMock;
    private readonly Mock<ICestaRecomendadaService> _cestaRecomendadaServiceMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly CriarCestaHandler _handler;

    public CriarCestaHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CriarCestaHandler>>();
        _cestaRecomendadaServiceMock = new Mock<ICestaRecomendadaService>();
        _clienteServiceMock = new Mock<IClienteService>();
        _handler = new(_loggerMock.Object, _cestaRecomendadaServiceMock.Object, _clienteServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CriarCestaRealizada()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var response = new CriarCestaRecomendadaDto { CestaAtualizada = false, CestaAtual = new CestaRecomendadaDto { CestaId = 1, Nome = "", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new List<ComposicaoCestaDto> { } }, CestaAnterior = null };
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.CriarCestaAsync(It.IsAny<CriarCestaRecomendadaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CriarCestaRecomendadaResponse>();
        resultado.Value.CestaAnteriorDesativada.Should().BeNull();

        _cestaRecomendadaServiceMock.Verify(s =>
            s.CriarCestaAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CriarCestaAtualizada()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var response = new CriarCestaRecomendadaDto { CestaAtualizada = true, CestaAtual = new CestaRecomendadaDto { CestaId = 2, Nome = "Cesta Test 2", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new List<ComposicaoCestaDto> { } }, CestaAnterior = new CestaRecomendadaDto { CestaId = 1, Nome = "Cesta Test", DataCriacao = DateTime.MinValue, DataDesativacao = DateTime.Now, Ativa = false, Itens = new List<ComposicaoCestaDto> { } } };
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.CriarCestaAsync(It.IsAny<CriarCestaRecomendadaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        _clienteServiceMock
            .Setup(x => x.QuantidadeClientesAtivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CriarCestaRecomendadaResponse>();
        resultado.Value.CestaAnteriorDesativada.Should().NotBeNull();

        _cestaRecomendadaServiceMock.Verify(s =>
            s.CriarCestaAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_CriarCestaFalhar()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var exception = new Exception("Erro na compra");
        var result = Result.Error<CriarCestaRecomendadaDto>(exception);

        _cestaRecomendadaServiceMock
            .Setup(s => s.CriarCestaAsync(It.IsAny<CriarCestaRecomendadaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_QuantidadeClientesAtivos_Falhar()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var response = new CriarCestaRecomendadaDto { CestaAtualizada = true, CestaAtual = new CestaRecomendadaDto { CestaId = 2, Nome = "Cesta Test 2", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new List<ComposicaoCestaDto> { } }, CestaAnterior = new CestaRecomendadaDto { CestaId = 1, Nome = "Cesta Test", DataCriacao = DateTime.MinValue, DataDesativacao = DateTime.Now, Ativa = false, Itens = new List<ComposicaoCestaDto> { } } };
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.CriarCestaAsync(It.IsAny<CriarCestaRecomendadaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        _clienteServiceMock
            .Setup(x => x.QuantidadeClientesAtivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Exception("Error"));

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeOfType<Exception>();
        resultado.Value.Should().BeNull();

        _cestaRecomendadaServiceMock.Verify(s =>
            s.CriarCestaAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}