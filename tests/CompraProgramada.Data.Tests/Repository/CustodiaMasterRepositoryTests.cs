using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Tests.Repository;

public class CustodiaMasterRepositoryTests : SqliteTestBase
{
    private readonly CustodiaMasterRepository _repo;

    public CustodiaMasterRepositoryTests() => _repo = new CustodiaMasterRepository(_context);

    [Fact]
    public async Task CriarAsync_Deve_Persistir_Custodias()
    {
        // Arrange
        ContaMaster conta = new(1, "MST-000001", DateTime.MinValue, new() { });
        List<CustodiaMaster> custodias = new()
        {
            new CustodiaMaster(1, 1, "PETR4", 3, conta)
        };

        // Act
        var result = await _repo.CriarAsync(custodias, CancellationToken.None);

        // Assert
        _context.CustodiaMaster.Should().HaveCount(1);
        result.Should().NotBeNull();
        result.First().QuantidadeResiduo.Should().Be(3);
    }

    [Fact]
    public async Task ObterResiduosAsync_Deve_RetornarVazio_Quando_NaoTiverResiduos()
    {
        // Arrange & Act
        var result = await _repo.ObterResiduosAsync(CancellationToken.None);

        // Assert
        _context.CustodiaMaster.Should().HaveCount(0);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterResiduosAsync_Deve_RetornarCustodias_Quando_TiverResiduos()
    {
        // Arrange
        ContaMaster conta = new(1, "MST-000001", DateTime.MinValue, new() { });
        List<CustodiaMaster> custodias = new()
        {
            new CustodiaMaster(1, 1, "PETR4", 3, conta)
        };

        _context.CustodiaMaster.AddRange(custodias);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterResiduosAsync(CancellationToken.None);

        // Assert
        _context.CustodiaMaster.Should().HaveCount(1);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AtualizarResiduosAsync_Deve_Atualizar_Custodias()
    {
        // Arrange
        ContaMaster conta = new(1, "MST-000001", DateTime.MinValue, new() { });
        List<CustodiaMaster> custodias = new()
        {
            new CustodiaMaster(1, 1, "PETR4", 3, conta)
        };

        _context.CustodiaMaster.AddRange(custodias);
        await _context.SaveChangesAsync();

        custodias.First().AtualizarResiduo(10, 9);

        // Act
        await _repo.AtualizarResiduosAysnc(custodias, CancellationToken.None);

        var custodiaAtualizada = await _context.CustodiaMaster
            .Include(x => x.ContaMaster)
            .ToListAsync();

        // Assert
        _context.CustodiaMaster.Should().HaveCount(1);
        custodiaAtualizada.First().QuantidadeResiduo.Should().Be(4);
    }
}