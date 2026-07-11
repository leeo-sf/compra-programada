using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions.Base;
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

public class MotorEndpointsTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IMediator _mediator;
    private const string MOTOR_ENDPOINT = "/api/motor/executar-compra";

    public MotorEndpointsTests(ApiWebApplicationFactory application)
    {
        _client = application.CreateClient();
        _mediator = application.MediatorMock;
    }

    [Fact]
    public async Task Dado_Request_ExecutarCompra_DeveRetornarSucesso200_QuandoSolicitado()
    {
        // Arrange
        var dataExecucao = DateTime.Now.AddDays(-2);

        var request = new ExecutarMotorCompraRequest(DateOnly.FromDateTime(dataExecucao));

        var responseContent = new ExecutarMotorCompraResponse(
            dataExecucao,
            1,
            100m,
            new List<OrdemCompraDto>(),
            new List<DistribuicaoDto>(),
            new List<AtivoQuantidadeDto>(),
            0,
            "ok"
        );

        _mediator
            .Send(Arg.Any<ExecutarMotorCompraRequest>())
            .Returns(Result.Success(responseContent));

        // Act
        var response = await _client
            .PostAsync(MOTOR_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        await _mediator.Received(1).Send(Arg.Any<ExecutarMotorCompraRequest>());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_Request_E_ApiRetornarException_Quando_ExecutarCompra_Deve_ChamarMediatr_E_RetornarErro()
    {
        // Arrange
        var request = new ExecutarMotorCompraRequest(DateOnly.FromDateTime(DateTime.Now));

        var erroMapeado = new DomainException("mensagem", "codigo");

        _mediator.Send(Arg.Any<ExecutarMotorCompraRequest>())
            .Returns(Result.Error<ExecutarMotorCompraResponse>(erroMapeado));

        // Act
        var response = await _client
            .PostAsync(MOTOR_ENDPOINT,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        await _mediator.Received().Send(Arg.Any<ExecutarMotorCompraRequest>());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}