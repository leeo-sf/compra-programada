using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using OperationResult;
using CompraProgramada.Domain.Contract.Service;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Handler;

public class MotorCompraHandlerTests
{
    private readonly ILogger<MotorCompraHandler> _logger;
    private readonly ICompraService _compraService;
    private readonly MotorCompraHandler _sut;

    public MotorCompraHandlerTests()
    {
        _logger = Substitute.For<ILogger<MotorCompraHandler>>();
        _compraService = Substitute.For<ICompraService>();
        _sut = new MotorCompraHandler(_logger, _compraService);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoCompraForExecutada()
    {
        var dataReferencia = new DateOnly(2024, 1, 1);
        var request = new ExecutarCompraRequest(DateTime.Now, dataReferencia);

        var response = new ExecutarCompraResponse(DateTime.Now, 1, 1,
            new List<OrdemCompraDto> { new OrdemCompraDto { Ticker = "", QuantidadeTotal = 1, Detalhes = new List<OrdemCompraDetalheDto> { new OrdemCompraDetalheDto { Ticker = "", Tipo = "", Quantidade = 1 } }, PrecoUnitario = 1 } },
            new List<DistribuicaoDto> (),
            new List<AtivoQuantidadeDto> { new AtivoQuantidadeDto { Ticker = "", Quantidade = 1 } },
            1, "");
        var result = Result.Success(response);

        _compraService
            .ExecutarCompraAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())!
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(response);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_QuandoServicoFalhar()
    {
        var dataReferencia = new DateOnly(2024, 1, 1);
        var request = new ExecutarCompraRequest(DateTime.Now, dataReferencia);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<ExecutarCompraResponse>(exception);

        _compraService
            .ExecutarCompraAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())!
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }
}