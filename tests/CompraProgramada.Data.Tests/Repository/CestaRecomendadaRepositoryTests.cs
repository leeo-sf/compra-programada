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
        _repo.Criar(cesta);

        await _context.SaveChangesAsync();

        // Assert
        _context.CestaRecomendada.Should().HaveCount(1);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Cesta()
    {
        // Arrange
        var cesta = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        _context.CestaRecomendada.Add(cesta);
        await _context.SaveChangesAsync();

        // Act
        cesta.DesativarCesta();
        _repo.Atualizar(cesta);
        await _context.SaveChangesAsync();

        // Assert
        _context.CestaRecomendada.Should().HaveCount(1);
        cesta.Ativa.Should().BeFalse();
    }

    [Fact]
    public async Task ObterCestaAsync_Deve_Buscar_Cesta_Ativa()
    {
        // Arrange
        var cesta = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });
        var cestaTwo = new CestaRecomendada("Name Two", false, new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        _context.CestaRecomendada.AddRange([cesta, cestaTwo]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterCestaAtualAsync(CancellationToken.None);

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
        var cestaTwo = new CestaRecomendada("Name Two", false, new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });

        _context.CestaRecomendada.AddRange([cesta, cestaTwo]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterCestasAsync(CancellationToken.None);

        // Assert
        _context.CestaRecomendada.Should().HaveCount(2);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyHaveUniqueItems(x => x.Ativa);
    }
}