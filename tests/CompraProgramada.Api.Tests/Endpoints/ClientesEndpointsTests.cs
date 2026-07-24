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

public class ClientesEndpointsTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IMediator _mediator = factory.MediatorMock;
    private const string ADESAO_ENDPOINT = "/api/clientes/adesao";
    private const string SAIDA_ENDPOINT = "/api/clientes/{id}/saida";
    private const string VALOR_MENSAL_ENDPOINT = "/api/clientes/{id}/valor-mensal";
    private const string CARTEIRA_ENDPOINT = "/api/clientes/{id}/carteira";
    private const string RENTABILIDADE_ENDPOINT = "/api/clientes/{id}/rentabilidade";

    [Fact]
    public async Task Dado_Request_Adesao_DeveRetornarSucesso200_QuandoSolicitado()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 100);

        var responseContent = new AdesaoResponse(
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

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .PostAsync(ADESAO_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarCpfExistenteException_Quando_AderirProduto_Deve_ChamarMediatr_E_RetornarBadRequest()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 100);

        CpfExistenteException erroMapeado = new();

        _mediator.Send(request)
            .Returns(Result.Error<AdesaoResponse>(erroMapeado));

        var response = await _client
            .PostAsync(ADESAO_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var result = TestUtils.ReadResultContentApi<ErroResponse>(await response.Content.ReadAsStringAsync());

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().BeEquivalentTo(new
        {
            Mensagem = "CPF já cadastrado no sistema",
            Codigo = "CLIENTE_CPF_DUPLICADO"
        });
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarValorMensalException_Quando_AderirProduto_Deve_ChamarMediatr_E_RetornarBadRequest()
    {
        var request = new AdesaoRequest("Teste", "11111111111", "email@teste.com", 99);

        ValorMensalException erroMapeado = new(100);

        _mediator.Send(request)
            .Returns(Result.Error<AdesaoResponse>(erroMapeado));

        var response = await _client
            .PostAsync(ADESAO_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var result = TestUtils.ReadResultContentApi<ErroResponse>(await response.Content.ReadAsStringAsync());

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().BeEquivalentTo(new
        {
            Mensagem = "O valor mensal mínimo é de R$ 100,00",
            Codigo = "VALOR_MENSAL_INVALIDO"
        });
    }

    [Fact]
    public async Task Dado_Request_SaidaProduto_DeveRetornarSucesso200_QuandoSolicitado()
    {
        var request = new SaidaProdutoRequest(1);

        var responseContent = new SaidaProdutoResponse(
            1,
            "Nome",
            false
        );

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .PostAsync(SAIDA_ENDPOINT.Replace("{id}", request.ClienteId.ToString()),
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarClienteNaoEncontradoException_Quando_SaidaProduto_Deve_ChamarMediatr_E_RetornarNotFound()
    {
        var request = new SaidaProdutoRequest(1);

        ClienteNaoEncontradoException erroMapeado = new();

        _mediator.Send(request)
            .Returns(Result.Error<SaidaProdutoResponse>(erroMapeado));

        var response = await _client
            .PostAsync(SAIDA_ENDPOINT.Replace("{id}", request.ClienteId.ToString()),
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var result = TestUtils.ReadResultContentApi<ErroResponse>(await response.Content.ReadAsStringAsync());

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Should().BeEquivalentTo(new
        {
            Mensagem = "Cliente não encontrado",
            Codigo = "CLIENTE_NAO_ENCONTRADO"
        });
    }

    [Fact]
    public async Task Dado_Request_AlterarValorMensal_DeveRetornarSucesso200_QuandoSolicitado()
    {
        var request = new AtualizarValorMensalRequest(1, 1000m);

        var responseContent = new AtualizarValorMensalResponse(
            1,
            100,
            1000
        );

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .PutAsync(VALOR_MENSAL_ENDPOINT.Replace("{id}", request.ClienteId.ToString()),
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarClienteNaoEncontradoException_Quando_AlterarValorMensal_Deve_ChamarMediatr_E_RetornarNotFound()
    {
        var request = new AtualizarValorMensalRequest(1, 1000);

        ClienteNaoEncontradoException erroMapeado = new();

        _mediator.Send(request)
            .Returns(Result.Error<AtualizarValorMensalResponse>(erroMapeado));

        var response = await _client
            .PutAsync(VALOR_MENSAL_ENDPOINT.Replace("{id}", request.ClienteId.ToString()),
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var result = TestUtils.ReadResultContentApi<ErroResponse>(await response.Content.ReadAsStringAsync());

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Should().BeEquivalentTo(new
        {
            Mensagem = "Cliente não encontrado",
            Codigo = "CLIENTE_NAO_ENCONTRADO"
        });
    }

    [Fact]
    public async Task Dado_Request_ConsultarCarteira_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new CarteiraCustodiaRequest(1);

        var responseContent = new CarteiraCustodiaResponse(
            1,
            "Nome",
            "conta",
            DateTime.Now,
            new ResumoCarteiraDto { ValorTotalInvestido = 100, ValorAtualCarteira = 80, PlTotal = 8.4m, RentabilidadePercentual = 0.90m },
            new List<DetalheCarteiraDto> { new DetalheCarteiraDto { Ticker = "Ticker", Quantidade = 10, PrecoMedio = 49, CotacaoAtual = 52, ValorAtual = 10, Pl = 10, PlPercentual = 10, ComposicaoCarteira = 10 } });

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .GetAsync(CARTEIRA_ENDPOINT.Replace("{id}", request.ClienteId.ToString()));

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_Request_ConsultarRentabilidade_DeveRetornarSucesso_QuandoSolicitado()
    {
        var request = new RentabilidadeRequest(1);

        var responseContent = new RentabilidadeResponse(
            1,
            "Nome",
            DateTime.Now,
            new ResumoCarteiraDto { ValorTotalInvestido = 100, ValorAtualCarteira = 80, PlTotal = 8.4m, RentabilidadePercentual = 0.90m },
            new List<HistoricoAporteDto> { new HistoricoAporteDto { Valor = 1000, Data = DateOnly.FromDateTime(DateTime.Now), Parcela = "1/3" } },
            new List<EvolucaoCarteiraDto> { new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(DateTime.Now), Rentabilidade = 1000, ValorCarteira = 1000, ValorInvestido = 100 } });

        _mediator.Send(request)
            .Returns(Result.Success(responseContent));

        var response = await _client
            .GetAsync(RENTABILIDADE_ENDPOINT.Replace("{id}", request.ClienteId.ToString()));

        await _mediator.Received().Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}