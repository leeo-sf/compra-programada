using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Handler;

public class AdministradorHandlerTests
{
    private readonly Mock<ILogger<AdministradorHandler>> _loggerMock;
    private readonly Mock<ICestaRecomendadaService> _cestaRecomendadaServiceMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly Mock<CestaRecomendadaMapper> _mapperMock;
    private readonly AdministradorHandler _handler;

    public AdministradorHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AdministradorHandler>>();
        _cestaRecomendadaServiceMock = new Mock<ICestaRecomendadaService>();
        _clienteServiceMock = new Mock<IClienteService>();
        _mapperMock = new Mock<CestaRecomendadaMapper>();
        _handler = new AdministradorHandler(_loggerMock.Object, _clienteServiceMock.Object, _cestaRecomendadaServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CriarCestaRealizada()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new("", 10) });

        var response = new CriarCestaRecomendadaDto(false, new(1, "", DateTime.Now, null, true, new List<ComposicaoCestaRecomendadaDto> { }), null);
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.CriarCestaAsync(It.IsAny<CriarCestaRecomendadaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CriarCestaRecomendadaResponse>();

        _cestaRecomendadaServiceMock.Verify(s =>
            s.CriarCestaAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_CriarCestaFalhar()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new("", 10) });

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
    public async Task Handle_DeveRetornarSucesso_Quando_CestaAtualConsultada()
    {
        var request = new CestaAtualRequest();

        var response = new CestaRecomandadaDto(1, "", DateTime.Now, null, true, new List<ComposicaoCestaRecomendadaDto> { });
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!
            .ReturnsAsync(Result.Success(response));

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CestaRecomendadaResponse>();

        _cestaRecomendadaServiceMock.Verify(s =>
            s.ObterCestaAtivaAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultarCestaAtualFalhar()
    {
        var request = new CestaAtualRequest();

        var exception = new Exception("Erro na compra");
        var result = Result.Error<CestaRecomandadaDto>(exception);

        _cestaRecomendadaServiceMock
            .Setup(s => s.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_HistoricoCestaConsultada()
    {
        var request = new CestaHistoricoRequest();

        var response = new List<CestaRecomandadaDto> { new(1, "", DateTime.Now, null, true, new List<ComposicaoCestaRecomendadaDto> { new(1, 1, "", 1) }) };
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.HistoricoCestasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(response));

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<List<CestaRecomendadaResponse>>();

        _cestaRecomendadaServiceMock.Verify(s =>
            s.HistoricoCestasAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultarHistoricoCestaFalhar()
    {
        var request = new CestaHistoricoRequest();

        var exception = new Exception("Erro na compra");
        var result = Result.Error<List<CestaRecomandadaDto>>(exception);

        _cestaRecomendadaServiceMock
            .Setup(s => s.HistoricoCestasAsync(It.IsAny<CancellationToken>()))!
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
    }
}