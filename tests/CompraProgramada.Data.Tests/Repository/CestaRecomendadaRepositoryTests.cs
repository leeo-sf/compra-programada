using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class CestaRecomendadaRepositoryTests : SqliteTestBase
{
    private readonly CestaRecomendadaRepository _repo;

    public CestaRecomendadaRepositoryTests() => _repo = new CestaRecomendadaRepository(_context);

    [Fact]
    public async Task CriarAsync_Deve_Persistir_Cesta()
    {
        // Arrange
        var cesta = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        // Act
        var result = await _repo.CriarAsync(cesta, CancellationToken.None);

        // Assert
        _context.CestaRecomendada.Should().HaveCount(1);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Cesta()
    {
        // Arrange
        var cesta = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });
        var cestaAtualizada = CestaRecomendada.CriarCesta("Name Updated", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        _context.CestaRecomendada.Add(cesta);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.AtualizarAsync(cestaAtualizada, CancellationToken.None);

        // Assert
        _context.CestaRecomendada.Should().HaveCount(1);
        result.Nome.Should().Be("NAME UPDATED");
    }

    [Fact]
    public async Task ObterCestaAsync_Deve_Buscar_Cesta_Ativa()
    {
        // Arrange
        var cesta = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });
        var cestaTwo = new CestaRecomendada(0, "Name Two", DateTime.MinValue, DateTime.MinValue, false, new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        _context.CestaRecomendada.AddRange([cesta, cestaTwo]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterCestaAtivaAsync(CancellationToken.None);

        // Assert
        _context.CestaRecomendada.Should().HaveCount(2);
        result.Should().NotBeNull();
        result.Ativa.Should().BeTrue();
        result.ComposicaoCesta.Should().NotBeNull();
    }

    [Fact]
    public async Task ObterCestasAsync_Deve_Buscar_TodasAsCestas()
    {
        // Arrange
        var cesta = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });
        var cestaTwo = new CestaRecomendada(0, "Name Two", DateTime.MinValue, DateTime.MinValue, false, new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        _context.CestaRecomendada.AddRange([cesta, cestaTwo]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterTodasCestasAsync(CancellationToken.None);

        // Assert
        _context.CestaRecomendada.Should().HaveCount(2);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyHaveUniqueItems(x => x.Ativa);
    }
}