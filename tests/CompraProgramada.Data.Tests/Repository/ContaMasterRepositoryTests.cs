using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class ContaMasterRepositoryTests : SqliteTestBase
{
    private readonly ContaMasterRepository _repo;

    public ContaMasterRepositoryTests() => _repo = new ContaMasterRepository(_context);

    [Fact]
    public async Task CriarAsync_Deve_Persistir_ContaMaster()
    {
        // Arrange
        var conta = ContaMaster.Gerar(1, new List<CustodiaMaster>());

        // Act
        var result = await _repo.CriarAsync(conta, CancellationToken.None);

        // Assert
        _context.ContaMaster.Should().HaveCount(1);
        result.Should().NotBeNull();
    }

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

    [Fact]
    public async Task AtualizarResiduosAsync_Deve_Atualizar_Custodias()
    {
        // Arrange
        var conta = ContaMaster.Gerar(1, new List<CustodiaMaster>() { CustodiaMaster.CriarCustodia(1, "PETR4") });

        _context.ContaMaster.Add(conta);
        await _context.SaveChangesAsync();

        var custodia = conta.CustodiaMasters.First();
        custodia.AtualizarResiduo(30, 10);

        // Act
        var result = await _repo.AtualizarResiduosAysnc(conta, CancellationToken.None);

        // Assert
        _context.ContaMaster.Should().HaveCount(1);
        result.CustodiaMasters.Should().HaveCount(1);
        result.CustodiaMasters.First().QuantidadeResiduo.Should().Be(20);
    }
}