using CompraProgramada.Application.Request;
using CompraProgramada.Application.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using CompraProgramada.Application.Interface;
using FluentAssertions;
using Moq;
using OperationResult;
using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Tests.Service;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<IContaGraficaService> _contaServiceMock;
    private readonly Mock<ICestaRecomendadaService> _cestaServiceMock;
    private readonly Mock<ICustodiaFilhoteService> _custodiaServiceMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _clienteRepoMock = new Mock<IClienteRepository>();
        _contaServiceMock = new Mock<IContaGraficaService>();
        _cestaServiceMock = new Mock<ICestaRecomendadaService>();
        _custodiaServiceMock = new Mock<ICustodiaFilhoteService>();

        _service = new ClienteService(_clienteRepoMock.Object, _contaServiceMock.Object, _cestaServiceMock.Object, _custodiaServiceMock.Object);
    }

    [Fact]
    public async Task AdesaoAsync_DeveRetornarErro_QuandoValorMenorQueMinimo()
    {
        var request = new AdesaoRequest("N", "111", "e@e", 50m);

        var result = await _service.AdesaoAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AdesaoAsync_DeveRetornarErro_QuandoCpfJaCadastrado()
    {
        var request = new AdesaoRequest("N", "111", "e@e", 150m);
        _clienteRepoMock.Setup(r => r.ExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.AdesaoAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AdesaoAsync_DeveRetornarErro_QuandoCestaNaoExistir()
    {
        var request = new AdesaoRequest("N", "111", "e@e", 150m);
        _clienteRepoMock.Setup(r => r.ExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        _cestaServiceMock.Setup(c => c.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Error<CestaRecomandadaDto>(new Exception("exception")));

        var result = await _service.AdesaoAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AdesaoAsync_DeveCriarCliente_QuandoTudoOK()
    {
        var request = new AdesaoRequest("Nome", "111", "e@e", 150m);
        _clienteRepoMock.Setup(r => r.ExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _cestaServiceMock.Setup(c => c.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Success(new CestaRecomandadaDto { Id = 1, Nome = "C1", DataCriacao = DateTime.UtcNow }));
        _clienteRepoMock.Setup(r => r.CriarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente c, CancellationToken ct) => c with { Id = 5 });
        _contaServiceMock.Setup(c => c.GerarContaGraficaAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(new ContaGraficaDto(1, "000", DateTime.UtcNow, 5, "Corrente", new List<HistoricoCompraDto>(), new List<CustodiaFilhoteDto>())));

        var result = await _service.AdesaoAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(5);
    }

    [Fact]
    public async Task SairDoProdutoAsync_DeveRetornarErro_QuandoClienteNaoExistir()
    {
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente)null);

        var result = await _service.SairDoProdutoAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SairDoProdutoAsync_DeveRetornarErro_QuandoClienteInativo()
    {
        var cliente = new Cliente(1, "N", "111", "e@e", 100m, 100m, DateTime.UtcNow) { Ativo = false };
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

        var result = await _service.SairDoProdutoAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SairDoProdutoAsync_DeveDesativarCliente_QuandoAtivo()
    {
        var cliente = new Cliente(1, "N", "111", "e@e", 100m, 100m, DateTime.UtcNow) { Ativo = true };
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _clienteRepoMock.Setup(r => r.AtualizarClienteAsync(It.IsAny<Cliente>(), It.IsAny<Cliente>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente a, Cliente b, CancellationToken ct) => b);

        var result = await _service.SairDoProdutoAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarValorMensalAsync_DeveRetornarErro_QuandoValorMenor()
    {
        var request = new AtualizarValorMensalRequest(1, 50m);

        var result = await _service.AtualizarValorMensalAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarValorMensalAsync_DeveRetornarErro_QuandoClienteNaoExistir()
    {
        var request = new AtualizarValorMensalRequest(1, 150m);
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente)null!);

        var result = await _service.AtualizarValorMensalAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarValorMensalAsync_DeveRetornarErro_QuandoClienteInativo()
    {
        var request = new AtualizarValorMensalRequest(1, 150m);
        var cliente = new Cliente(1, "N", "111", "e@e", 100m, 100m, DateTime.UtcNow) { Ativo = false };
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

        var result = await _service.AtualizarValorMensalAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarValorMensalAsync_DeveAtualizar_QuandoValido()
    {
        var request = new AtualizarValorMensalRequest(1, 200m);
        var cliente = new Cliente(1, "N", "111", "e@e", 100m, 100m, DateTime.UtcNow) { Ativo = true };
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _clienteRepoMock.Setup(r => r.AtualizarClienteAsync(It.IsAny<Cliente>(), It.IsAny<Cliente>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente a, Cliente b, CancellationToken ct) => b);

        var result = await _service.AtualizarValorMensalAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ValorMensal.Should().Be(200m);
    }

    [Fact]
    public async Task ConsultarCarteiraAsync_DeveRetornarErro_QuandoClienteNaoExistir()
    {
        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente)null);

        var result = await _service.ConsultarCarteiraAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ConsultarCarteiraAsync_DeveRetornarErro_QuandoServicoCustodiaFalhar()
    {
        var cliente = new Cliente(1, "N", "111", "e@e", 100m, 100m, DateTime.UtcNow)
        {
            ContaGrafica = new ContaGrafica(1, "000", DateTime.UtcNow, 1) { CustodiaFilhotes = new List<CustodiaFilhote>() }
        };

        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _custodiaServiceMock.Setup(c => c.ObterRentabilidadeDaCertira(It.IsAny<List<CustodiaFilhoteDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<CarteiraDto>(new Exception("erro")));

        var result = await _service.ConsultarCarteiraAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ConsultarCarteiraAsync_DeveRetornarSucesso_QuandoTudoOk()
    {
        var cliente = new Cliente(1, "N", "111", "e@e", 100m, 100m, DateTime.UtcNow)
        {
            ContaGrafica = new ContaGrafica(1, "000", DateTime.UtcNow, 1) { CustodiaFilhotes = new List<CustodiaFilhote>() }
        };

        _clienteRepoMock.Setup(r => r.ObterClienteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        var resumo = new ResumoCarteiraDto(100m, 120m, 20m, 20m);
        var ativos = new List<DetalheAtivoCarteiraDto>();
        _custodiaServiceMock.Setup(c => c.ObterRentabilidadeDaCertira(It.IsAny<List<CustodiaFilhoteDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(new CarteiraDto(resumo, ativos)));

        var result = await _service.ConsultarCarteiraAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(1);
    }
}
