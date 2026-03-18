using CompraProgramada.Api.Controllers;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OperationResult;
using System.Net;

namespace CompraProgramada.Api.Tests.Controllers;

public class ClienteControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ClienteController _sut;

    public ClienteControllerTests() => _sut = new ClienteController(_mediator);

    [Fact]
    public async Task Adesao_DeveRetornarSucesso_QuandoSolicitado()
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
    public async Task Dado_UmaRequest_E_ApiRetornarCpfExistenteException_Quando_AderirProduto_Deve_ChamarMediatr_E_RetornarBadRequest()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 100);
        CpfExistenteException erroMapeado = new();

        _mediator.Send(request).Returns(Result.Error<AdesaoResponse>(erroMapeado));

        var result = await _sut.AdesaoAsync(request);

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "CPF ja cadastrado no sistema.", Codigo = "CLIENTE_CPF_DUPLICADO" })
            { StatusCode = (int)HttpStatusCode.BadRequest },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarValorMensalException_Quando_AderirProduto_Deve_ChamarMediatr_E_RetornarBadRequest()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 99);
        ValorMensalException erroMapeado = new(100);

        _mediator.Send(request).Returns(Result.Error<AdesaoResponse>(erroMapeado));

        var result = await _sut.AdesaoAsync(request);

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "O valor mensal minimo e de R$ 100,00", Codigo = "VALOR_MENSAL_INVALIDO" })
            { StatusCode = (int)HttpStatusCode.BadRequest },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task SaidaProduto_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new SaidaProdutoRequest(1);
        var response = new SaidaProdutoResponse(
            1,
            "Nome",
            false
        );

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.SaidaProdutoAsync(1);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarClienteNaoEncontradoException_Quando_SaidaProduto_Deve_ChamarMediatr_E_RetornarNotFound()
    {
        var request = new SaidaProdutoRequest(1);
        ClienteNaoEncontradoException erroMapeado = new();

        _mediator.Send(request).Returns(Result.Error<SaidaProdutoResponse>(erroMapeado));

        var result = await _sut.SaidaProdutoAsync(1);

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "Cliente nao encontrado.", Codigo = "CLIENTE_NAO_ENCONTRADO" })
            { StatusCode = (int)HttpStatusCode.NotFound },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task AlterarValorMensal_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new AtualizarValorMensalRequest(1, 1000m);
        var response = new AtualizarValorMensalResponse(
            1,
            100,
            1000
        );

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.AlterarValorMensalAsync(1, new AtualizarValorMensalRequest(1, 1000));

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarClienteNaoEncontradoException_Quando_AlterarValorMensal_Deve_ChamarMediatr_E_RetornarNotFound()
    {
        var request = new AtualizarValorMensalRequest(1, 1000);
        ClienteNaoEncontradoException erroMapeado = new();

        _mediator.Send(request).Returns(Result.Error<AtualizarValorMensalResponse>(erroMapeado));

        var result = await _sut.AlterarValorMensalAsync(1, new AtualizarValorMensalRequest(1, 1000));

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "Cliente nao encontrado.", Codigo = "CLIENTE_NAO_ENCONTRADO" })
            { StatusCode = (int)HttpStatusCode.NotFound },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task ConsultarCarteira_DeveRetornarSucesso_QuandoSolicitado()
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
    public async Task ConsultarRentabilidade_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new RentabilidadeRequest(1);
        var response = new RentabilidadeResponse(
            1,
            "Nome",
            DateTime.Now,
            new(1000, 1000, 10, 10),
            new List<HistoricoAporteDto> { new HistoricoAporteDto(1, DateOnly.FromDateTime(DateTime.Now), 1000, "1/3") },
            new List<EvolucaoCarteiraDto> { new EvolucaoCarteiraDto(1, DateOnly.FromDateTime(DateTime.Now), 1000, 1000, 100) });

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.RentabilidadeAsync(1);

        await _mediator.Received().Send(request);
    }
}