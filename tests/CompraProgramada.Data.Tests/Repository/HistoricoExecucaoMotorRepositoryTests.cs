using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class HistoricoExecucaoMotorRepositoryTests : SqliteTestBase
{
    private readonly HistoricoExecucaoMotorRepository _repo;

    public HistoricoExecucaoMotorRepositoryTests() => _repo = new HistoricoExecucaoMotorRepository(_context);

    [Fact]
    public async Task ObterExecucaoRealizadaAsync_Deve_RetornarNull_Quando_NaoTiverExecuacoNaData()
    {
        // Arrange
        var execucao = HistoricoExecucaoMotor.CriarRegistroHistorico(new DateTime(2026, 05, 10), new DateTime(2026, 05, 11));

        _context.HistoricoExecucaoMotor.Add(execucao);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObtemExecucaoRealizadaAsync(new DateTime(2026, 05, 10), CancellationToken.None);

        // Assert
        _context.HistoricoExecucaoMotor.Should().HaveCount(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ObterExecucaoRealizadaAsync_Deve_RetornarExecucao_Quando_TiverExecuacoNaData()
    {
        // Arrange
        var execucao = HistoricoExecucaoMotor.CriarRegistroHistorico(new DateTime(2026, 05, 10), new DateTime(2026, 05, 11));

        _context.HistoricoExecucaoMotor.Add(execucao);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObtemExecucaoRealizadaAsync(new DateTime(2026, 05, 11), CancellationToken.None);

        // Assert
        _context.HistoricoExecucaoMotor.Should().HaveCount(1);
        result.Should().NotBeNull();
        result.Executado.Should().BeTrue();
    }

    [Fact]
    public async Task CriarHistoricoExecucaoAsync_Deve_Persistir_HistoricoExecucaoMotor()
    {
        // Arrange
        var execucao = HistoricoExecucaoMotor.CriarRegistroHistorico(new DateTime(2026, 05, 10), new DateTime(2026, 05, 11));

        // Act
        await _repo.CriarHistoricoExecucaoAsync(execucao, CancellationToken.None);

        // Assert
        _context.HistoricoExecucaoMotor.Should().HaveCount(1);
    }
}