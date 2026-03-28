using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class OrdemCompraRepositoryTests : SqliteTestBase
{
    private readonly OrdemCompraRepository _repo;

    public OrdemCompraRepositoryTests() => _repo = new OrdemCompraRepository(_context);

    [Fact]
    public async Task ObterOrdensCompraAsync_Deve_RetornarNull_Quando_NaoTiverOrdensCompra()
    {
        // Arrange
        List<OrdemCompra> ordensCompra = new()
        {
            OrdemCompra.GerarOrdemCompra("PETR4", 100, 35),
            OrdemCompra.GerarOrdemCompra("ITUB4", 52, 62)
        };

        _context.OrdemCompra.AddRange(ordensCompra);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterOrdensCompraAsync(DateTime.Now.AddDays(-1), CancellationToken.None);

        // Assert
        _context.OrdemCompra.Should().HaveCount(2);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterOrdensCompraAsync_Deve_RetornarOrdensCompra_Quando_TiverOrdensCompra()
    {
        // Arrange
        List<OrdemCompra> ordensCompra = new()
        {
            OrdemCompra.GerarOrdemCompra("PETR4", 100, 35),
            OrdemCompra.GerarOrdemCompra("ITUB4", 52, 62)
        };

        _context.OrdemCompra.AddRange(ordensCompra);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterOrdensCompraAsync(DateTime.Now, CancellationToken.None);

        // Assert
        _context.OrdemCompra.Should().HaveCount(2);
        result.Should().NotBeNull();
        result.Should().NotContainNulls();
    }

    [Fact]
    public async Task SalvarOrdensDeCompra_Deve_Persistir_OrdensCompra()
    {
        // Arrange
        List<OrdemCompra> ordensCompra = new()
        {
            OrdemCompra.GerarOrdemCompra("PETR4", 100, 35),
            OrdemCompra.GerarOrdemCompra("ITUB4", 52, 62)
        };

        // Act
        var result = await _repo.SalvarOrdensDeCompra(ordensCompra, CancellationToken.None);

        // Assert
        _context.OrdemCompra.Should().HaveCount(2);
        result.Should().NotBeNull();
        result.Should().NotContainNulls();
    }
}