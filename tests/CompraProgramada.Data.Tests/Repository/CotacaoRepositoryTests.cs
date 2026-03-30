using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class CotacaoRepositoryTests : SqliteTestBase
{
    private readonly CotacaoRepository _repo;

    public CotacaoRepositoryTests() => _repo = new CotacaoRepository(_context);

    [Fact]
    public async Task ObterCotacaoAsync_Deve_RetornarNull_Quando_CotacaoNaoExistir_Na_Data()
    {
        // Arrange
        var cotacao = Cotacao.CriarRegistro(DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 37) });

        _context.Cotacao.Add(cotacao);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterCotacaoAsync(DateTime.Now.AddDays(-1), CancellationToken.None);

        // Assert
        _context.Cotacao.Should().HaveCount(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ObterCotacaoAsync_Deve_RetornarCotacao_Quando_CotacaoExistir_Na_Data()
    {
        // Arrange
        var cotacao = Cotacao.CriarRegistro(DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 37) });

        _context.Cotacao.Add(cotacao);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterCotacaoAsync(DateTime.Now, CancellationToken.None);

        // Assert
        _context.Cotacao.Should().HaveCount(1);
        result.Should().NotBeNull();
        result.DataPregao.Should().Be(cotacao.DataPregao);
    }

    [Fact]
    public async Task SalvarCotacaoAsync_Deve_Persistir_Cotacao()
    {
        // Arrange
        var cotacao = Cotacao.CriarRegistro(DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 37) });

        // Act
        var result = await _repo.SalvarCotacaoAsync(cotacao, CancellationToken.None);

        // Assert
        _context.Cotacao.Should().HaveCount(1);
        result.Should().NotBeNull();
        result.DataPregao.Should().Be(cotacao.DataPregao);
    }
}