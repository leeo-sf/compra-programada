using CompraProgramada.Api.Controllers;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using NSubstitute;
using OperationResult;

namespace CompraProgramada.Api.Tests.Controllers;

public class ClienteControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ClienteController _sut;

    public ClienteControllerTests() => _sut = new ClienteController(_mediator);

    [Fact]
    public async Task Deve_Retornar_Sucesso_AoAderirAoProduto_Quando_Mediator_RetornaSucesso()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 100);
        var response = new AdesaoResponse(
            1,
            request.Nome,
            request.Cpf,
            request.Email,
            request.ValorMensal,
            true,
            DateTime.Now,
            new ContaGraficaResponse(
                1,
                "number",
                "FILHOTE",
                DateTime.Now
            )
        );

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.AdesaoAsync(request);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_AoAderirAoProduto_Quando_Mediator_RetornaErro()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 100);
        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<AdesaoResponse>(erroMapeado));

        await _sut.AdesaoAsync(request);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Sucesso_AoConsultarCarteira_Quando_Mediator_RetornaSucesso()
    {
        var request = new CarteiraCustodiaRequest(1);
        var response = new CarteiraCustodiaResponse(
            1,
            "Nome",
            "conta",
            DateTime.Now,
            new ResumoCarteiraDto(100, 80, 8.4m, 0.90m),
            new List<DetalheAtivoCarteiraDto> { new DetalheAtivoCarteiraDto("Ticker", 10, 49, 52, 10, 10, 10, 10) });

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.CustodiaCarteiraAsync(1);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_AoConsultarCarteira_Quando_Mediator_RetornaErro()
    {
        var request = new CarteiraCustodiaRequest(1);
        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<CarteiraCustodiaResponse>(erroMapeado));

        await _sut.CustodiaCarteiraAsync(1);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Sucesso_AoSairDoProduto_Quando_Mediator_RetornaSucesso()
    {
        var request = new SaidaProdutoRequest(1);
        var response = new SaidaProdutoResponse
        {
            ClienteId = 1,
            Nome = "Nome",
            Ativo = false,
            DataSaida = DateTime.Now,
            Mensagem = "Adesão encerrada"
        };

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.SaidaProdutoAsync(1);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_AoSairDoProduto_Quando_Mediator_RetornaErro()
    {
        var request = new SaidaProdutoRequest(1);
        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<SaidaProdutoResponse>(erroMapeado));

        await _sut.SaidaProdutoAsync(1);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Sucesso_AoAlterarValorMensal_Quando_Mediator_RetornaSucesso()
    {
        var request = new AtualizarValorMensalRequest(1, 1000m);
        var response = new AtualizarValorMensalResponse
        {
            ClienteId = 1,
            ValorMensalAnterior = 100,
            ValorMensalNovo = 1000
        };

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.AlterarValorMensalAsync(1, new AtualizarValorMensalRequest(1, 1000));

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_AoAlterarValorMensal_Quando_Mediator_RetornaErro()
    {
        var request = new AtualizarValorMensalRequest(1, 1000);
        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<AtualizarValorMensalResponse>(erroMapeado));

        await _sut.AlterarValorMensalAsync(1, new AtualizarValorMensalRequest(1, 1000));

        await _mediator.Received().Send(request);
    }
}