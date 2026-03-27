using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class HistoricoExecucaoMotorServiceTests
{
    private readonly IHistoricoExecucaoMotorRepository _historicoExecucaoRepository;
    private readonly ICalendarioMotorCompraService _calendarioCompraService;
    private readonly HistoricoExecucaoMotorService _sut;

    public HistoricoExecucaoMotorServiceTests()
    {
        _historicoExecucaoRepository = Substitute.For<IHistoricoExecucaoMotorRepository>();
        _calendarioCompraService = Substitute.For<ICalendarioMotorCompraService>();
        _sut = new(_historicoExecucaoRepository, _calendarioCompraService);
    }

    [Fact]
    public async Task HistoricoExecucaoMotor_Deve_Retornar_True_Quando_AindaNaoFoiExecutadoCompra()
    {
        // Arrange
        _calendarioCompraService.DeveExecutarCompraHoje()
            .Returns(true);

        _historicoExecucaoRepository.ObtemExecucaoRealizadaAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((HistoricoExecucaoMotor)null!);

        // Act
        var result = await _sut.ExecutarCompraHojeAsync(CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HistoricoExecucaoMotor_Deve_Retornar_False_Quando_EhDiaDeExecutar_MasJaFoiExecutado()
    {
        // Arrange
        _calendarioCompraService.DeveExecutarCompraHoje()
            .Returns(true);

        _historicoExecucaoRepository.ObtemExecucaoRealizadaAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(HistoricoExecucaoMotor.CriarRegistroHistorico(DateTime.Now, DateTime.Now));

        // Act
        var result = await _sut.ExecutarCompraHojeAsync(CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HistoricoExecucaoMotor_Deve_Retornar_False_Quando_NaoEhDiaDeExecutar()
    {
        // Arrange
        _calendarioCompraService.DeveExecutarCompraHoje()
            .Returns(false);

        // Act
        var result = await _sut.ExecutarCompraHojeAsync(CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}