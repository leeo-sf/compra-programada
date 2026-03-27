using CompraProgramada.Application.Config;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using FluentAssertions;

namespace CompraProgramada.Application.Tests.Service;

public class CalendarioMotorCompraServiceTests
{
    private readonly AppConfig _config;

    public CalendarioMotorCompraServiceTests() => _config = AppConfigHelper.GetAppConfig();

    [Theory]
    [InlineData("2026-01-15", true)]
    [InlineData("2026-02-15", false)]
    [InlineData("2026-02-11", false)]
    [InlineData("2026-03-25", true)]
    [InlineData("2026-05-05", true)]
    public void Deve_Retornar_SeEhDiaDeCompra_Quando_DeveExecutarCompraHoje_Solicitado(string dataAtual, bool deveExecutarCompraResult)
    {
        // Arrange
        var dateTimeProvaiderFaker = new DateTimeProvaiderHelper(DateTime.Parse(dataAtual));
        var sut = new CalendarioMotorCompraService(_config, dateTimeProvaiderFaker);

        // Act
        var result = sut.DeveExecutarCompraHoje();

        // Assert
        result.Should().Be(deveExecutarCompraResult);
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
        var sut = new CalendarioMotorCompraService(_config, dateTimeProvaiderFaker);

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
        var sut = new CalendarioMotorCompraService(_config, dateTimeProvaiderFaker);

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
        var sut = new CalendarioMotorCompraService(_config);

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
        var sut = new CalendarioMotorCompraService(_config);

        // Act
        var result = sut.ObterProximoDiaUtil(DateTime.Parse(data));

        // Assert
        result.Date.Should().Be(DateTime.Parse(proximoDiaUtil).Date);
    }
}