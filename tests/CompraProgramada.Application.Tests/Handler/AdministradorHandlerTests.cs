using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

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

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CestaAtualConsultada()
    {
        var request = new CestaAtualRequest();

        var itensCesta = FakerRequest.ComposicaoCestaRecomendada();
        var response = CestaRecomendada.CriarCesta("Name", itensCesta.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());
        var result = Result.Success(response);

        _cestaRecomendadaServiceMock
            .Setup(s => s.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!
            .ReturnsAsync(Result.Success(response));

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CestaRecomendadaDto>();

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
        var result = Result.Error<CestaRecomendada>(exception);

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

        var response = new List<CestaRecomendada> { };

        _cestaRecomendadaServiceMock
            .Setup(s => s.HistoricoCestasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(response));

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<HistoricoCestasResponse>();

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
        var result = Result.Error<List<CestaRecomendada>>(exception);

        _cestaRecomendadaServiceMock
            .Setup(s => s.HistoricoCestasAsync(It.IsAny<CancellationToken>()))!
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
    }
}