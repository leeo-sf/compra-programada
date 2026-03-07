using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Handler;

public class ClienteHandlerTests
{
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly ClienteHandle _handler;

    public ClienteHandlerTests()
    {
        _clienteServiceMock = new Mock<IClienteService>();
        var logger = NullLogger<ClienteHandle>.Instance;
        _handler = new ClienteHandle(logger, _clienteServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Adesao_DeveRetornarSucesso()
    {
        var request = new AdesaoRequest("Nome Teste", "12345678900", "email@teste.com", 150m);

        var cliente = new ClienteDto
        {
            ClienteId = 10,
            Nome = "Nome Teste",
            Cpf = "12345678900",
            Email = "email@teste.com",
            ValorAnterior = 0m,
            ValorMensal = 150m,
            Ativo = true,
            DataAdesao = DateTime.UtcNow,
            ContaGrafica = new ContaGraficaDto(1, "0001-1", DateTime.UtcNow, 10, "Corrente", new List<HistoricoCompraDto>(), new List<CustodiaFilhoteDto>())
        };

        _clienteServiceMock.Setup(s => s.AdesaoAsync(It.IsAny<AdesaoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(cliente));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(cliente.ClienteId);
        result.Value.Nome.Should().Be(cliente.Nome);
        result.Value.Cpf.Should().Be(cliente.Cpf);
        result.Value.Email.Should().Be(cliente.Email);
        result.Value.ValorMensal.Should().Be(cliente.ValorMensal);
        result.Value.ContaGrafica.Id.Should().Be(cliente.ContaGrafica.Id);
    }

    [Fact]
    public async Task Handle_Adesao_DeveRetornarErro_QuandoServicoFalhar()
    {
        var request = new AdesaoRequest("Nome Teste", "12345678900", "email@teste.com", 150m);
        var exception = new Exception("Erro no servico");

        _clienteServiceMock.Setup(s => s.AdesaoAsync(It.IsAny<AdesaoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<ClienteDto>(exception));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_SaidaProduto_DeveRetornarSucesso()
    {
        var request = new SaidaProdutoRequest(5);
        var cliente = new ClienteDto
        {
            ClienteId = 5,
            Nome = "Cliente Saida",
            Cpf = "000",
            Email = "",
            ValorAnterior = 100m,
            ValorMensal = 100m,
            Ativo = false,
            DataAdesao = DateTime.UtcNow,
            ContaGrafica = new ContaGraficaDto(2, "0002-2", DateTime.UtcNow, 5, "Corrente", new List<HistoricoCompraDto>(), new List<CustodiaFilhoteDto>())
        };

        _clienteServiceMock.Setup(s => s.SairDoProdutoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(cliente));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(cliente.ClienteId);
        result.Value.Nome.Should().Be(cliente.Nome);
    }

    [Fact]
    public async Task Handle_SaidaProduto_DeveRetornarErro_QuandoServicoFalhar()
    {
        var request = new SaidaProdutoRequest(5);
        var exception = new Exception("Erro SairProduto");

        _clienteServiceMock.Setup(s => s.SairDoProdutoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<ClienteDto>(exception));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_AtualizarValorMensal_DeveRetornarSucesso()
    {
        var request = new AtualizarValorMensalRequest(7, 200m);

        var cliente = new ClienteDto
        {
            ClienteId = 7,
            Nome = "Cliente Atualiza",
            Cpf = "111",
            Email = "",
            ValorAnterior = 150m,
            ValorMensal = 200m,
            Ativo = true,
            DataAdesao = DateTime.UtcNow,
            ContaGrafica = new ContaGraficaDto(3, "0003-3", DateTime.UtcNow, 7, "Corrente", new List<HistoricoCompraDto>(), new List<CustodiaFilhoteDto>())
        };

        _clienteServiceMock.Setup(s => s.AtualizarValorMensalAsync(It.IsAny<AtualizarValorMensalRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(cliente));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(cliente.ClienteId);
        result.Value.ValorMensalAnterior.Should().Be(cliente.ValorAnterior);
        result.Value.ValorMensalNovo.Should().Be(cliente.ValorMensal);
    }

    [Fact]
    public async Task Handle_AtualizarValorMensal_DeveRetornarErro_QuandoServicoFalhar()
    {
        var request = new AtualizarValorMensalRequest(7, 200m);
        var exception = new Exception("Erro atualizar valor");

        _clienteServiceMock.Setup(s => s.AtualizarValorMensalAsync(It.IsAny<AtualizarValorMensalRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<ClienteDto>(exception));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_ConsultarCarteira_DeveRetornarSucesso()
    {
        var request = new CarteiraCustodiaRequest(9);

        var carteira = new CarteiraCustodiaResponse(
            9,
            "Nome Carteira",
            "0009-9",
            DateTime.UtcNow,
            new ResumoCarteiraDto(100m, 120m, 20m, 20m),
            new List<DetalheAtivoCarteiraDto>());

        _clienteServiceMock.Setup(s => s.ConsultarCarteiraAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(carteira));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClienteId.Should().Be(9);
        result.Value.Nome.Should().Be("Nome Carteira");
    }

    [Fact]
    public async Task Handle_ConsultarCarteira_DeveRetornarErro_QuandoServicoFalhar()
    {
        var request = new CarteiraCustodiaRequest(9);
        var exception = new Exception("Erro consultar carteira");

        _clienteServiceMock.Setup(s => s.ConsultarCarteiraAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<CarteiraCustodiaResponse>(exception));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }
}
