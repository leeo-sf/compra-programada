using CompraProgramada.Shared.Config;
using CompraProgramada.Domain.Service;
using CompraProgramada.Domain.Tests.TestUtils;
using FluentAssertions;
using CompraProgramada.Domain.Contract.Repository;
using NSubstitute;
using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Tests.Service;

public class CalendarioMotorCompraServiceTests
{
    private readonly AppConfig _config;
    private readonly IHistoricoExecucaoMotorRepository _historicoExecucaoMotorRepository;

    public CalendarioMotorCompraServiceTests()
    {
        _config = AppConfigHelper.GetAppConfig();
        _historicoExecucaoMotorRepository = Substitute.For<IHistoricoExecucaoMotorRepository>();
    }

    [Theory]
    [InlineData("2026-01-15", false, true)]
    [InlineData("2026-01-15", true, false)]
    [InlineData("2026-02-15", false, false)]
    [InlineData("2026-02-11", false, false)]
    [InlineData("2026-03-25", true, false)]
    [InlineData("2026-05-05", false, true)]
    public async Task Deve_Retornar_SeEhDiaDeCompra_Quando_DeveExecutarCompraHoje_Solicitado(string dataAtual, bool compraJaFoiExecutada, bool deveExecutarCompraValorEsperado)
    {
        // Arrange
        var dateTimeProvaiderFaker = new DateTimeProvaiderHelper(DateTime.Parse(dataAtual));
        var sut = new CalendarioMotorCompraService(_config, dateTimeProvaiderFaker, _historicoExecucaoMotorRepository);

        _historicoExecucaoMotorRepository.ObterHistoricoExecucaoAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(compraJaFoiExecutada ? HistoricoExecucaoMotor.CriarRegistroHistorico(DateTime.MinValue, DateTime.MinValue) : null);

        // Act
        var result = await sut.DeveExecutarCompraHoje(CancellationToken.None);

        // Assert
        result.Should().Be(deveExecutarCompraValorEsperado);
    }

    [Theory]
    [InlineData("2026-01-08", "2026-01-15")]
    [InlineData("2026-02-10", "2026-02-16")]
    [InlineData("2026-03-05", "2026-03-16")]
    [InlineData("2026-03-31", "2026-04-06")]
    [InlineData("2026-04-17", "2026-04-27")]
    [InlineData("2025-12-30", "2026-01-05")]
    public void Deve_Retornar_ProximaDataCompra_Quando_Solicitado(string dataAtual, string proximaDataCompra)
    {
        // Arrange
        var dateTimeProvaiderFaker = new DateTimeProvaiderHelper(DateTime.Parse(dataAtual));
        var sut = new CalendarioMotorCompraService(_config, dateTimeProvaiderFaker, default!);

        // Act
        var result = sut.ObterProximaDataCompra();

        // Assert
        result.Date.Should().Be(DateTime.Parse(proximaDataCompra).Date);
    }

    [Theory]
    [InlineData("2026-03-05", "2026-03-05", "2026-03-05")]
    [InlineData("2026-02-16", "2026-02-16", "2026-02-15")]
    [InlineData("2026-04-28", "2026-04-27", "2026-04-25")]
    [InlineData("2026-04-06", "2026-04-06", "2026-04-05")]
    public void Deve_Retornar_DataReferenciaExecucao_Quando_Solicitado(string dataAtual, string dataExecutada, string dataReferencia)
    {
        // Arrange
        var dateTimeProvaiderFaker = new DateTimeProvaiderHelper(DateTime.Parse(dataAtual));
        var sut = new CalendarioMotorCompraService(_config, dateTimeProvaiderFaker, default!);

        // Act
        var result = sut.ObterDataReferenciaExecucao(DateTime.Parse(dataExecutada));

        // Assert
        result.Date.Should().Be(DateTime.Parse(dataReferencia).Date);
    }

    [Theory]
    [InlineData("2026-03-25", true)]
    [InlineData("2026-03-15", false)]
    [InlineData("2026-03-05", true)]
    [InlineData("2026-03-07", false)]
    public void Deve_Retornar_SeEhDiaUtil_Quando_Solicitado(string data, bool ehDiaUtil)
    {
        // Arrange
        var sut = new CalendarioMotorCompraService(_config, default!, default!);

        // Act
        var result = sut.EhDiaUtil(DateTime.Parse(data));

        // Assert
        result.Should().Be(ehDiaUtil);
    }

    [Theory]
    [InlineData("2026-03-25", "2026-03-25")]
    [InlineData("2026-03-15", "2026-03-16")]
    [InlineData("2026-03-05", "2026-03-05")]
    [InlineData("2026-03-07", "2026-03-09")]
    public void Deve_Retornar_ProximoDiaUtil_Quando_Solicitado(string data, string proximoDiaUtil)
    {
        // Arrange
        var sut = new CalendarioMotorCompraService(_config, default!, default!);

        // Act
        var result = sut.ObterProximoDiaUtil(DateTime.Parse(data));

        // Assert
        result.Date.Should().Be(DateTime.Parse(proximoDiaUtil).Date);
    }
}