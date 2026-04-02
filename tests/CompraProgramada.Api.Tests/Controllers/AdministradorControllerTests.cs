using CompraProgramada.Api.Controllers;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using CompraProgramada.Shared.Dto;
using OperationResult;
using NSubstitute;
using CompraProgramada.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CompraProgramada.Api.Tests.Controllers;

public class AdministradorControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly AdministradorController _sut;

    public AdministradorControllerTests() => _sut = new AdministradorController(_mediator);

    [Fact]
    public async Task CriarCesta_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new CriarCestaRecomendadaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto { Ticker = "PETR4", Percentual = 30 },
                new ComposicaoCestaDto { Ticker = "VALE3", Percentual = 25 },
                new ComposicaoCestaDto{ Ticker = "ITUB4", Percentual = 20 },
                new ComposicaoCestaDto{ Ticker = "BBDC4", Percentual = 15 }
            }
        );

        _mediator.Send(request).Returns(Result.Success(new CriarCestaRecomendadaResponse(default, default!, default, default, default!, default, default, default, default, default!)));

        await _sut.CestaAsync(request);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarQuantidadeItensCestaException_Quando_CriarCesta_Deve_ChamarMediatr_E_RetornarBadRequest()
    {
        var request = new CriarCestaRecomendadaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto{ Ticker = "PETR4", Percentual = 30 },
                new ComposicaoCestaDto{ Ticker = "VALE3", Percentual = 25 }
            }
        );
        QuantidadeItensCestaException error = new(3);

        _mediator.Send(request).Returns(Result.Error<CriarCestaRecomendadaResponse>(error));

        var result = await _sut.CestaAsync(request);

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "A cesta deve conter exatamente 5 ativos. Quantidade informada: 3.", Codigo = "QUANTIDADE_ATIVOS_INVALIDA" })
            { StatusCode = (int)HttpStatusCode.BadRequest },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarPercentualCestaException_Quando_CriarCesta_Deve_ChamarMediatr_E_RetornarBadRequest()
    {
        var request = new CriarCestaRecomendadaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto{ Ticker = "PETR4", Percentual = 30 },
                new ComposicaoCestaDto{ Ticker = "VALE3", Percentual = 25 }
            }
        );
        PercentualCestaException error = new(98);

        _mediator.Send(request).Returns(Result.Error<CriarCestaRecomendadaResponse>(error));

        var result = await _sut.CestaAsync(request);

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "A soma dos percentuais deve ser exatamente 100%. Soma atual: 98%.", Codigo = "PERCENTUAIS_INVALIDOS" })
            { StatusCode = (int)HttpStatusCode.BadRequest },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task CestaAtual_DeveRetornarSucesso_QuandoSolicitado()
    {
        var response = new CestaRecomendadaDto { CestaId = 1, Nome = "Name", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new() };

        _mediator.Send(new CestaAtualRequest()).Returns(Result.Success(response));

        await _sut.CestaAtualAsync();

        await _mediator.Received().Send(new CestaAtualRequest());
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarNenhumaCestaAtiva_Quando_ConsultarCestaAtual_Deve_ChamarMediatr_E_RetornarUnprocessableEntity()
    {
        var request = new CestaAtualRequest();
        ApplicationException error = new("Nenhuma cesta cadastrada.");

        _mediator.Send(request).Returns(Result.Error<CestaRecomendadaDto>(error));

        var result = await _sut.CestaAtualAsync();

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Message = "Nenhuma cesta cadastrada." })
            { StatusCode = (int)HttpStatusCode.UnprocessableEntity },
            Assert.IsType<ObjectResult>(result.Result));
    }

    [Fact]
    public async Task HistoricoCesta_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new CestaHistoricoRequest();

        var response = new HistoricoCestasResponse(
            new List<CestaRecomendadaDto> { new CestaRecomendadaDto { CestaId = 1, Nome = "Name", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new() } }
        );

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.HistoricoAsync();

        await _mediator.Received().Send(request);
    }
}