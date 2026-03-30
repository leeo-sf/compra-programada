using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class CustodiaFilhoteRepositoryTests : SqliteTestBase
{
    private readonly CustodiaFilhoteRepository _repo;

    public CustodiaFilhoteRepositoryTests() => _repo = new CustodiaFilhoteRepository(_context);

    [Fact]
    public async Task AtualizarCustodiasAsync_Deve_Atualizar_Custodias()
    {
        // Arrange
        Cliente cliente = new(1, "Name", "11111111111", "email@test.com", 100, 100, true, DateTime.MinValue);
        ContaGrafica conta = new(1, "FLh-000001", DateTime.MinValue, cliente, new() { }, new() { new CustodiaFilhote(1, 1, "PETR4", 35, 10) }, new() { });

        _context.ContaGrafica.Add(conta);
        await _context.SaveChangesAsync();

        conta.CustodiaFilhotes.First().AdicionarNovaQuantidade(15);

        // Act
        var result = await _repo.AtualizarCustodiasAsync(conta.CustodiaFilhotes, CancellationToken.None);

        // Assert
        _context.CustodiaFilhote.Should().HaveCount(1);
        result.Should().NotBeNull();
        result.First().Quantidade.Should().Be(25);
    }
}