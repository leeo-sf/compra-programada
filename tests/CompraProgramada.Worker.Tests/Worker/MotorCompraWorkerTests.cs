using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Shared.Config;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Worker.Tests.TestUtils;
using CompraProgramada.Worker.Worker;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CompraProgramada.Worker.Tests.Worker;

public class MotorCompraWorkerTests
{
    private readonly ILogger<MotorCompraWorker> _logger;
    private readonly AppConfig _appConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;
    private readonly MotorCompraWorker _sut;

    public MotorCompraWorkerTests()
    {
        _logger = Substitute.For<ILogger<MotorCompraWorker>>();
        _appConfig = AppConfigHelper.GetAppConfig();
        _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        _mediator = Substitute.For<IMediator>();
        _sut = new(_logger, _appConfig, _serviceScopeFactory);
    }

    [Fact]
    public async Task Deve_ExecutarCompra_ComSucesso_Quando_Iteracao_Executar()
    {
        // Arrange
        _mediator.Send(Arg.Any<ExecutarMotorCompraRequest>(), Arg.Any<CancellationToken>())
            .Returns((ExecutarMotorCompraResponse)null!);

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(IMediator))
            .Returns(_mediator);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        _serviceScopeFactory.CreateScope().Returns(scope);

        // Act
        await _sut.ExecutarMotorDeCompra(CancellationToken.None);

        // Assert
        await _mediator.Received(1)
            .Send(Arg.Any<ExecutarMotorCompraRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deve_Falhar_Quando_ExecutarCompra_RetornarException()
    {
        // Arrange
        _mediator.Send(Arg.Any<ExecutarMotorCompraRequest>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception());

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(IMediator))
            .Returns(_mediator);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        _serviceScopeFactory.CreateScope().Returns(scope);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _sut.ExecutarMotorDeCompra(CancellationToken.None));
    }
}