using CompraProgramada.Api.Controllers;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using NSubstitute;
using OperationResult;
using System.Net;

namespace CompraProgramada.Api.Tests.Controllers;

public class MotorControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly MotorController _sut;

    public MotorControllerTests() => _sut = new MotorController(_mediator);

    [Fact]
    public async Task Deve_Retornar_Sucesso_Quando_Mediator_RetornaSucesso()
    {
        var dataAtual = DateTime.Now;
        var dataExecucao = dataAtual.AddDays(2);

        var request = new ExecutarCompraRequest(dataAtual, DateOnly.FromDateTime(dataExecucao));
        var response = new ExecutarCompraResponse(
            dataExecucao,
            1,
            100m,
            new List<OrdemCompraDto>(),
            new List<DistribuicaoDto>(),
            new List<ResiduoCustodiaMasterDto>(),
            0,
            "ok"
        );

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.ExecutarCompraAsync(request);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Deve_Retornar_ErroMapeado_Quando_Mediator_RetornaErro()
    {
        var request = new ExecutarCompraRequest(DateTime.Now, DateOnly.FromDateTime(DateTime.Now));
        var erroMapeado = new ErroMapeadoException("bad", "CODE", HttpStatusCode.InternalServerError);

        _mediator.Send(request).Returns(Result.Error<ExecutarCompraResponse>(erroMapeado));

        await _sut.ExecutarCompraAsync(request);

        await _mediator.Received().Send(request);
    }
}