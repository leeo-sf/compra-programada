using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Handler;

public class MotorCompraHandlerTests
{
    private readonly Mock<ICompraService> _compraServiceMock;
    private readonly MotorCompraHandler _handler;

    public MotorCompraHandlerTests()
    {
        _compraServiceMock = new Mock<ICompraService>();
        var logger = NullLogger<MotorCompraHandler>.Instance;
        _handler = new MotorCompraHandler(logger, _compraServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoServicoExecutarComSucesso()
    {
        var request = new ExecutarCompraRequest(DateTime.Now, DateOnly.FromDateTime(DateTime.Now));
        var response = new ExecutarCompraResponse(
            DateTime.Now,
            1,
            100m,
            new List<OrdemCompraDto>(),
            new List<DistribuicaoDto>(),
            new List<ResiduoCustodiaMasterDto>(),
            0,
            "ok"
        );

        _compraServiceMock
            .Setup(s => s.ExecutarCompraAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(response));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(response);
        _compraServiceMock.Verify(s => s.ExecutarCompraAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoServicoFalhar()
    {
        var request = new ExecutarCompraRequest(DateTime.Now, DateOnly.FromDateTime(DateTime.Now));
        var exception = new Exception("Erro ao processar compra");

        _compraServiceMock
            .Setup(s => s.ExecutarCompraAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<ExecutarCompraResponse>(exception));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }
}