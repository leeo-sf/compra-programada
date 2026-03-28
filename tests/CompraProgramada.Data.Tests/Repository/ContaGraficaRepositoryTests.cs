using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Data.Tests.Repository;

public class ContaGraficaRepositoryTests : SqliteTestBase
{
    private readonly ContaGraficaRepository _repo;

    public ContaGraficaRepositoryTests() => _repo = new ContaGraficaRepository(_context);

    [Fact]
    public async Task CriarAsync_Deve_Persistir_ContaGrafica()
    {
        // Arrange
        var cliente = new Cliente(1, "Name", "11111111111", "email@mail.com", 100, 100, true, DateTime.MinValue);
        var conta = new ContaGrafica(cliente);

        // Act
        var result = await _repo.CriarAsync(conta, CancellationToken.None);

        // Assert
        _context.ContaGrafica.Should().HaveCount(1);
    }

    [Fact]
    public async Task AtualizarContasAsync_Deve_Atualizar_ContasGrafica()
    {
        // Arrange
        var cliente = new Cliente(1, "Name", "11111111111", "email@mail.com", 100, 100, true, DateTime.MinValue);
        var conta = new ContaGrafica(cliente);

        _context.ContaGrafica.Add(conta);
        await _context.SaveChangesAsync();

        conta.AdicionarDistribuicao(Distribuicao.CriarDistribuicao(10, conta, OrdemCompra.GerarOrdemCompra("PETR4", 100, 42)));
        
        // Act
        var result = await _repo.AtualizarContasAsync([conta], CancellationToken.None);

        // Assert
        _context.ContaGrafica.Should().HaveCount(1);
        result.Should().HaveCount(1);
    }
}