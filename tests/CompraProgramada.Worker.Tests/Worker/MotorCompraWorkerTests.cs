using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Response;
using CompraProgramada.Worker.Tests.TestUtils;
using CompraProgramada.Worker.Worker;
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
    private readonly ICompraService _compraService;
    private readonly MotorCompraWorker _sut;

    public MotorCompraWorkerTests()
    {
        _logger = Substitute.For<ILogger<MotorCompraWorker>>();
        _appConfig = AppConfigHelper.GetAppConfig();
        _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        _compraService = Substitute.For<ICompraService>();
        _sut = new(_logger, _appConfig, _serviceScopeFactory);
    }

    [Fact]
    public async Task Deve_ExecutarCompra_ComSucesso_Quando_Iteracao_Executar()
    {
        // Arrange
        _compraService.ExecutarCompraAsync(Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns((ExecutarCompraResponse)null!);

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(ICompraService))
            .Returns(_compraService);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        _serviceScopeFactory.CreateScope().Returns(scope);

        // Act
        await _sut.ExecutarMotorDeCompra(CancellationToken.None);

        // Assert
        await _compraService.Received(1)
            .ExecutarCompraAsync(null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deve_Falhar_Quando_ExecutarCompra_RetornarException()
    {
        // Arrange
        _compraService.ExecutarCompraAsync(Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception());

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(ICompraService))
            .Returns(_compraService);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        _serviceScopeFactory.CreateScope().Returns(scope);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _sut.ExecutarMotorDeCompra(CancellationToken.None));
    }
}