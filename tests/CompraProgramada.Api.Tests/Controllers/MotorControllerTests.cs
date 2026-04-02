using CompraProgramada.Api.Controllers;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions.Base;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public async Task ExecutarCompra_DeveRetornarSucesso_QuandoSolicitado()
    {
        var dataAtual = DateTime.Now;
        var dataExecucao = dataAtual.AddDays(-2);

        var request = new ExecutarCompraRequest(dataAtual, DateOnly.FromDateTime(dataExecucao));
        var response = new ExecutarCompraResponse(
            dataExecucao,
            1,
            100m,
            new List<OrdemCompraDto>(),
            new List<DistribuicaoDto>(),
            new List<AtivoQuantidadeDto>(),
            0,
            "ok"
        );

        _mediator.Send(request).Returns(Result.Success(response));

        await _sut.ExecutarCompraAsync(request);

        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task Dado_UmaRequest_E_ApiRetornarException_Quando_ExecutarCompra_Deve_ChamarMediatr_E_RetornarErro()
    {
        var request = new ExecutarCompraRequest(DateTime.Now, DateOnly.FromDateTime(DateTime.Now));
        var erroMapeado = new DomainException("mensagem", "codigo");

        _mediator.Send(request).Returns(Result.Error<ExecutarCompraResponse>(erroMapeado));

        var result = await _sut.ExecutarCompraAsync(request);

        await _mediator.Received().Send(request);
        Assert.Equivalent(
            new ObjectResult(new { Mensagem = "mensagem", Codigo = "codigo" })
            { StatusCode = (int)HttpStatusCode.BadRequest },
            Assert.IsType<ObjectResult>(result.Result));
    }
}