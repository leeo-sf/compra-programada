using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class HistoricoExecucaoMotorTests
{
    [Theory]
    [InlineData("2026-03-05", "2026-03-05")]
    [InlineData("2024-07-15", "2024-07-16")]
    public async Task CriarRegistroHistorico_DeveRetornarHistoricoComSucesso_Quando_DadosValidosInformados(string dataReferencia, string dataExecucao)
    {
        var historico = HistoricoExecucaoMotor.CriarRegistroHistorico(DateTime.Parse(dataReferencia), DateTime.Parse(dataExecucao));

        historico.Id.Should().Be(0);
        historico.DataReferencia.Should().Be(DateTime.Parse(dataReferencia));
        historico.DataExecucao.Should().Be(DateTime.Parse(dataExecucao));
        historico.Executado.Should().NotBe(false);
    }
}