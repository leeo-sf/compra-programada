using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Tests.Handler;

public class MotorCompraHandlerTests
{
    private readonly Mock<ILogger<MotorCompraHandler>> _loggerMock;
    private readonly Mock<ICompraService> _compraServiceMock;
    private readonly MotorCompraHandler _handler;

    public MotorCompraHandlerTests()
    {
        _loggerMock = new Mock<ILogger<MotorCompraHandler>>();
        _compraServiceMock = new Mock<ICompraService>();
        _handler = new MotorCompraHandler(_loggerMock.Object, _compraServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoCompraForExecutada()
    {
        var dataReferencia = new DateOnly(2024, 1, 1);
        var request = new ExecutarCompraRequest(DateTime.Now, dataReferencia);

        var response = new ExecutarCompraResponse(DateTime.Now, 1, 1,
            new List<OrdemCompraDto> { new OrdemCompraDto { Ticker = "", QuantidadeTotal = 1, Detalhes = new List<OrdemCompraDetalheDto> { new OrdemCompraDetalheDto { Ticker = "", Tipo = "", Quantidade = 1 } }, PrecoUnitario = 1 } },
            new List<DistribuicaoDto> (),
            new List<AtivoQuantidadeDto> { new AtivoQuantidadeDto { Ticker = "", Quantidade = 1 } },
            1, "");
        var result = Result.Success(response);

        _compraServiceMock
            .Setup(s => s.ExecutarCompraAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(response);

        _compraServiceMock.Verify(s =>
            s.ExecutarCompraAsync(
                dataReferencia.ToDateTime(TimeOnly.MinValue),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_QuandoServicoFalhar()
    {
        var dataReferencia = new DateOnly(2024, 1, 1);
        var request = new ExecutarCompraRequest(DateTime.Now, dataReferencia);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<ExecutarCompraResponse>(exception);

        _compraServiceMock
            .Setup(s => s.ExecutarCompraAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(result);

        var resultado = await _handler.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }
}