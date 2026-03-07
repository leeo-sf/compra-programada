using CompraProgramada.Application.Config;
using CompraProgramada.Application.Service;
using FluentAssertions;

namespace CompraProgramada.Application.Tests.Service;

public class CalendarioMotorCompraServiceTests
{
    [Fact]
    public void DeveExecutarCompraHoje_DeveRetornarTrue_QuandoDiaConfiguradoEDiaUtil()
    {
        var config = new AppConfig { MotorCompra = new MotorCompra { DiasDeCompra = new[] { 16 } } };
        var dataAtual = new DateTime(2026, 3, 16);

        var service = new CalendarioMotorCompraService(config, dataAtual);

        var result = service.DeveExecutarCompraHoje();

        result.Should().BeTrue();
    }

    [Fact]
    public void ObterProximaDataCompra_RetornaProximaDataNoMesmoMes_QuandoExisteDiaMaiorNoMes()
    {
        var config = new AppConfig { MotorCompra = new MotorCompra { DiasDeCompra = new[] { 5, 20 } } };
        var dataAtual = new DateTime(2026, 3, 4);

        var service = new CalendarioMotorCompraService(config, dataAtual);

        var proxima = service.ObterProximaDataCompra();

        proxima.Should().Be(new DateTime(2026, 3, 5));
    }

    [Fact]
    public void ObterProximaDataCompra_RetornaPrimeiroDiaProximoMes_QuandoTodosDiasJaPassaram()
    {
        var config = new AppConfig { MotorCompra = new MotorCompra { DiasDeCompra = new[] { 1, 10 } } };
        var dataAtual = new DateTime(2026, 3, 15);

        var service = new CalendarioMotorCompraService(config, dataAtual);

        var proxima = service.ObterProximaDataCompra();

        proxima.Should().Be(new DateTime(2026, 4, 1));
    }

    [Fact]
    public void ObterDataReferenciaExecucao_RetornaMesAnterior_QuandoDataAntesDoPrimeiroDia()
    {
        var config = new AppConfig { MotorCompra = new MotorCompra { DiasDeCompra = new[] { 5, 10 } } };
        var dataAtual = new DateTime(2026, 3, 3);

        var service = new CalendarioMotorCompraService(config, dataAtual);

        var referencia = service.ObterDataReferenciaExecucao(new DateTime(2026, 3, 3));

        referencia.Should().Be(new DateTime(2026, 2, 10));
    }

    [Fact]
    public void ObterDataReferenciaExecucao_RetornaDiaAnteriorDaLista_QuandoDataEntreDias()
    {
        var config = new AppConfig { MotorCompra = new MotorCompra { DiasDeCompra = new[] { 5, 10, 20 } } };
        var dataAtual = new DateTime(2026, 3, 12);

        var service = new CalendarioMotorCompraService(config, dataAtual);

        var referencia = service.ObterDataReferenciaExecucao(new DateTime(2026, 3, 12));

        referencia.Should().Be(new DateTime(2026, 3, 10));
    }
}
