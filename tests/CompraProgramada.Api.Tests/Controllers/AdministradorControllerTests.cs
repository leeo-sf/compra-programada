using System.Net;
using CompraProgramada.Api.Controllers;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using CompraProgramada.Application.Dto;
using OperationResult;
using NSubstitute;

namespace CompraProgramada.Api.Tests.Controllers;

public class AdministradorControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly AdministradorController _sut;

    public AdministradorControllerTests() => _sut = new AdministradorController(_mediator);

    [Fact]
    public async Task Deve_Retornar_Erro_AoCriarCesta_Quando_Mediator_RetornaErro()
    {
        var request = new CriarCestaRecomendadaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto("PETR4", 30),
                new ComposicaoCestaDto("VALE3", 25),
                new ComposicaoCestaDto("ITUB4", 20),
                new ComposicaoCestaDto("BBDC4", 15)
            }
        );

        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<CriarCestaRecomendadaResponse>(erroMapeado));

        await _sut.CestaAsync(request);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Sucesso_AoConsultarCestaAtual_Quando_Mediator_RetornaSucesso()
    {
        var request = new CestaAtualRequest();

        var response = new CestaRecomendadaResponse
        {
            CestaId = 1,
            Nome = "Cesta Top Five",
            Ativa = true,
            DataCriacao = DateTime.Now,
            Itens = new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto("PETR4", 30),
                new ComposicaoCestaDto("VALE3", 25),
                new ComposicaoCestaDto("ITUB4", 20),
                new ComposicaoCestaDto("BBDC4", 15),
                new ComposicaoCestaDto("WEGE3", 10)
            }
        };

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.CestaAtualAsync();

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_AoConsultarCestaAtual_Quando_Mediator_RetornaErro()
    {
        var request = new CestaAtualRequest();

        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<CestaRecomendadaResponse>(erroMapeado));

        await _sut.CestaAtualAsync();

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Sucesso_AoConsultarHistorico_Quando_Mediator_RetornaSucesso()
    {
        var request = new CestaHistoricoRequest();

        var response = new List<CestaRecomendadaResponse>
        {
            new CestaRecomendadaResponse
            {
                CestaId = 1,
                Nome = "Cesta Top Five",
                Ativa = true,
                DataCriacao = DateTime.Now,
                Itens = new List<ComposicaoCestaDto>
                {
                    new ComposicaoCestaDto("PETR4", 30),
                    new ComposicaoCestaDto("VALE3", 25),
                    new ComposicaoCestaDto("ITUB4", 20),
                    new ComposicaoCestaDto("BBDC4", 15),
                    new ComposicaoCestaDto("WEGE3", 10)
                }
            }
        };

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.HistoricoAsync();

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_AoConsultarHistoricol_Quando_Mediator_RetornaErro()
    {
        var request = new CestaHistoricoRequest();

        var erroMapeado = new Exception("bad");

        _mediator.Send(request).Returns(Result.Error<List<CestaRecomendadaResponse>>(erroMapeado));

        await _sut.HistoricoAsync();

        await _mediator.Received().Send(request);
    }
}