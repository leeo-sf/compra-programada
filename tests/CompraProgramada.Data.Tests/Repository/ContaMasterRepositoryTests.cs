using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class ContaMasterRepositoryTests : SqliteTestBase
{
    private readonly ContaMasterRepository _repo;

    public ContaMasterRepositoryTests() => _repo = new ContaMasterRepository(_context);

    [Fact]
    public async Task ObterContaMasterAsync_Deve_RetornarConta_Quando_Existir()
    {
        // Arrange
        var conta = ContaMaster.Gerar(1, new List<CustodiaMaster>());

        _context.ContaMaster.Add(conta);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterContaMasterAsync(CancellationToken.None);

        // Assert
        _context.ContaMaster.Should().HaveCount(1);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ObterContaMasterAsync_Deve_RetornarContaNull_Quando_NaoExistir()
    {
        // Arrange & Act
        var result = await _repo.ObterContaMasterAsync(CancellationToken.None);

        // Assert
        _context.ContaMaster.Should().HaveCount(0);
        result.Should().BeNull();
    }
}