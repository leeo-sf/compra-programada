using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using MediatR;
using NSubstitute;
using OperationResult;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CompraProgramada.Api.Tests.Endpoints;

public class AdministratorEndpointsTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IMediator _mediator = factory.MediatorMock;
    private const string CRIAR_CESTA_ENDPOINT = "/api/admin/cesta";
    private const string CESTA_ATUAL_ENDPOINT = "/api/admin/cesta/atual";
    private const string CESTA_HISTORICO_ENDPOINT = "/api/admin/cesta/historico";

    [Fact]
    public async Task Dado_Request_CriarCesta_DeveRetornarSucesso200_QuandoSucesso()
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

        _mediator.Send(Arg.Any<CriarCestaRecomendadaRequest>())
            .Returns(Result.Success(
                new CriarCestaRecomendadaResponse(default, default!, default, default, default!, default, default, default, default, default!)));

        var response = await _client
            .PostAsync(CRIAR_CESTA_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        await _mediator.Received().Send(Arg.Any<CriarCestaRecomendadaRequest>());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarQuantidadeItensCestaException_Quando_CriarCesta_Deve_RetornarBadRequest()
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

        _mediator.Send(Arg.Any<CriarCestaRecomendadaRequest>())
            .Returns(Result.Error<CriarCestaRecomendadaResponse>(error));

        var response = await _client
            .PostAsync(CRIAR_CESTA_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();

        await _mediator.Received().Send(Arg.Any<CriarCestaRecomendadaRequest>());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().BeEquivalentTo(JsonSerializer.Serialize(new
        {
            Mensagem = "A cesta deve conter exatamente 5 ativos. Quantidade informada: 3.",
            Codigo = "QUANTIDADE_ATIVOS_INVALIDA"
        }));
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

        _mediator.Send(Arg.Any<CriarCestaRecomendadaRequest>())
            .Returns(Result.Error<CriarCestaRecomendadaResponse>(error));

        var response = await _client
            .PostAsync(CRIAR_CESTA_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();

        await _mediator.Received().Send(Arg.Any<CriarCestaRecomendadaRequest>());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().BeEquivalentTo(JsonSerializer.Serialize(new
        {
            Mensagem = "A soma dos percentuais deve ser exatamente 100%. Soma atual: 98%.",
            Codigo = "PERCENTUAIS_INVALIDOS"
        }));
    }

    [Fact]
    public async Task Dado_Request_CestaAtual_DeveRetornarSucesso200_QuandoSolicitado()
    {
        var request = new CestaAtualRequest();

        var responseContent = new CestaRecomendadaDto { CestaId = 1, Nome = "Name", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new() };

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .GetAsync(CESTA_ATUAL_ENDPOINT);

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarNenhumaCestaAtiva_Quando_ConsultarCestaAtual_Deve_ChamarMediatr_E_RetornarUnprocessableEntity()
    {
        var request = new CestaAtualRequest();

        ApplicationException error = new("Nenhuma cesta cadastrada.");

        _mediator.Send(request)
            .Returns(Result.Error<CestaRecomendadaDto>(error));

        var response = await _client
            .GetAsync(CESTA_ATUAL_ENDPOINT);

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Dado_Request_HistoricoCesta_DeveRetornarSucesso200_QuandoSolicitado()
    {
        var request = new CestaHistoricoRequest();

        var responseContent = new HistoricoCestasResponse(
            new List<CestaRecomendadaDto> { new CestaRecomendadaDto { CestaId = 1, Nome = "Name", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new() } }
        );

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .GetAsync(CESTA_HISTORICO_ENDPOINT);

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}