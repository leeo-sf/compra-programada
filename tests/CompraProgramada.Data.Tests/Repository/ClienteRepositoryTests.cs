using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Data.Tests.Repository;

public class ClienteRepositoryTests : SqliteTestBase
{
    private readonly ClienteRepository _repo;

    public ClienteRepositoryTests() => _repo = new ClienteRepository(_context);

    [Fact]
    public async Task CriarAsync_Deve_Persistir_Cliente()
    {
        // Arrange
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150));

        // Act
        var result = await _repo.CriarAsync(cliente, CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(1);
        result.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task CriarAsync_Deve_Fahar_Quando_CpfExistir()
    {
        // Arrange
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150));

        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        var clienteB = Cliente.Criar(new("Name B", "11111111111", "email_b@teste.com", 500));

        // Act
        var act = () => _repo.CriarAsync(clienteB, CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(1);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Theory]
    [InlineData("11111111111", true)]
    [InlineData("11111111112", false)]
    public async Task ExisteAsync_Deve_Retornar_True_Quando_CpfExistir(string cpf, bool deveExistir)
    {
        // Arrange
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150));

        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ExisteAsync(cpf, CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(1);
        result.Should().Be(deveExistir);
    }

    [Fact]
    public async Task AtualizarClienteAsync_Deve_Atualizar_Cliente()
    {
        // Arrange
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150));

        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        cliente.AtualizarValorMensal(new(0, 200));

        // Act
        var result = await _repo.AtualizarClienteAsync(cliente, CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(1);
        result.Ativo.Should().BeTrue();
        result.ValorAnterior.Should().Be(150);
        result.ValorMensal.Should().Be(200);
    }

    [Fact]
    public async Task QuantidadeAtivosAsync_Deve_RetornarQtd_Clientes_Ativos()
    {
        // Arrange
        var clienteA = new Cliente(0, "Name A", "11111111111", "email_a@mail.com", 100, 100, true, DateTime.MinValue);
        var clienteB = new Cliente(0, "Name B", "11111111112", "email_b@mail.com", 100, 100, false, DateTime.MinValue);
        var clienteC = new Cliente(0, "Name C", "11111111113", "email_c@mail.com", 100, 100, true, DateTime.MinValue);

        _context.Cliente.AddRange([clienteA, clienteB, clienteC]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.QuantidadeAtivosAsync(CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(3);
        result.Should().Be(2);
    }

    [Fact]
    public async Task ObterClientesAtivosAsync_Deve_Retornar_Clientes_Ativos()
    {
        // Arrange
        var clienteA = new Cliente(0, "Name A", "11111111111", "email_a@mail.com", 100, 100, true, DateTime.MinValue);
        var clienteB = new Cliente(0, "Name B", "11111111112", "email_b@mail.com", 100, 100, false, DateTime.MinValue);
        var clienteC = new Cliente(0, "Name C", "11111111113", "email_c@mail.com", 100, 100, true, DateTime.MinValue);

        _context.Cliente.AddRange([clienteA, clienteB, clienteC]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterClientesAtivosAsync(CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(3);
        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.Ativo);
    }

    [Fact]
    public async Task ObterClienteAsync_Deve_Retornar_Cliente_QuandoExistir()
    {
        // Arrange
        var cliente = new Cliente(1, "Name A", "11111111111", "email_a@mail.com", 100, 100, true, DateTime.MinValue);

        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.ObterClienteAsync(cliente.Id, CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(1);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ObterClienteAsync_Deve_Retornar_Null_Quando_ClienteNaoExistir()
    {
        // Arrange & Act
        var result = await _repo.ObterClienteAsync(1, CancellationToken.None);

        // Assert
        _context.Cliente.Should().HaveCount(0);
        result.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_Deve_Persistir_ContaGrafica()
    {
        // Arrange
        var cliente = new Cliente(1, "Name", "11111111111", "email@mail.com", 100, 100, true, DateTime.MinValue);
        var conta = new ContaGrafica(cliente);

        // Act
        var result = await _repo.CriarContaAsync(conta, CancellationToken.None);

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