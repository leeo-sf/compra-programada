using CompraProgramada.Application.Handler;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Tests.Handler;

public class ClienteHandlerTests
{
    private readonly Mock<ILogger<ClienteHandle>> _loggerMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly Mock<ClienteMapper> _mapperMock;
    private readonly ClienteHandle _handler;

    public ClienteHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ClienteHandle>>();
        _clienteServiceMock = new Mock<IClienteService>();
        _mapperMock = new Mock<ClienteMapper>(Substitute.For<ContaMapper>());
        _handler = new ClienteHandle(_loggerMock.Object, _clienteServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_AdesaoRealizada()
    {
        var request = new AdesaoRequest("", "", "", 1);

        var response = FakerRequest.ClienteAtivo().Generate();
        response.AdicionarConta(ContaGrafica.Gerar(response));
        var result = Result.Success(response);

        _clienteServiceMock
            .Setup(s => s.RealizarAdesaoAsync(It.IsAny<AdesaoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<AdesaoResponse>();

        _clienteServiceMock.Verify(s =>
            s.RealizarAdesaoAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_RealizarAdesaoRetornarErro()
    {
        var request = new AdesaoRequest("", "", "", 1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<Cliente>(exception);

        _clienteServiceMock
            .Setup(s => s.RealizarAdesaoAsync(It.IsAny<AdesaoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_SaidaProdutoEfetuada()
    {
        var request = new SaidaProdutoRequest(1);

        var response = FakerRequest.ClienteAtivo().Generate();
        var result = Result.Success(response);

        _clienteServiceMock
            .Setup(s => s.SairDoProdutoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<SaidaProdutoResponse>();

        _clienteServiceMock.Verify(s =>
            s.SairDoProdutoAsync(
                1,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_SaidaProdutoRetornarErro()
    {
        var request = new SaidaProdutoRequest(1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<Cliente>(exception);

        _clienteServiceMock
            .Setup(s => s.SairDoProdutoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_ValorMensalAtualizado()
    {
        var request = new AtualizarValorMensalRequest(1, 100);

        var response = FakerRequest.ClienteAtivo().Generate();
        var result = Result.Success(response);

        _clienteServiceMock
            .Setup(s => s.AtualizarValorMensalAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<AtualizarValorMensalResponse>();

        _clienteServiceMock.Verify(s =>
            s.AtualizarValorMensalAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_AtualizarValorMensalRetornarErro()
    {
        var request = new AtualizarValorMensalRequest(1, 100);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<Cliente>(exception);

        _clienteServiceMock
            .Setup(s => s.AtualizarValorMensalAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CarteiraConsultada()
    {
        var request = new CarteiraCustodiaRequest(1);

        var response = new CarteiraCustodiaResponse(1, "", "", DateTime.Now, new ResumoCarteiraDto { ValorTotalInvestido = 100, ValorAtualCarteira = 80, PlTotal = 8.4m, RentabilidadePercentual = 0.90m }, new List<DetalheCarteiraDto> { });
        var result = Result.Success(response);

        _clienteServiceMock
            .Setup(s => s.ConsultarCarteiraAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CarteiraCustodiaResponse>();

        _clienteServiceMock.Verify(s =>
            s.ConsultarCarteiraAsync(
                It.Is<int>(x => x == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultarCarteiraRetornarErro()
    {
        var request = new CarteiraCustodiaRequest(1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<CarteiraCustodiaResponse>(exception);

        _clienteServiceMock
            .Setup(s => s.ConsultarCarteiraAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_RentabilidadeConsultada()
    {
        var request = new RentabilidadeRequest(1);

        var response = new RentabilidadeResponse(1, "", DateTime.Now, new ResumoCarteiraDto { ValorTotalInvestido = 100, ValorAtualCarteira = 80, PlTotal = 8.4m, RentabilidadePercentual = 0.90m }, new List<HistoricoAporteDto> { }, new List<EvolucaoCarteiraDto> { });
        var result = Result.Success(response);

        _clienteServiceMock
            .Setup(s => s.ConsultarRentabilidadeAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<RentabilidadeResponse>();

        _clienteServiceMock.Verify(s =>
            s.ConsultarRentabilidadeAsync(
                It.Is<int>(x => x == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultaRentabilidadeRetornarErro()
    {
        var request = new RentabilidadeRequest(1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<RentabilidadeResponse>(exception);

        _clienteServiceMock
            .Setup(s => s.ConsultarRentabilidadeAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }
}