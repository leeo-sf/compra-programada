using CompraProgramada.Application.Handler;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Domain.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OperationResult;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Domain.Contract.Service;

namespace CompraProgramada.Application.Tests.Handler;

public class ClienteHandlerTests
{
    private readonly ILogger<ClienteHandle> _loggerMock;
    private readonly IClienteService _clienteService;
    private readonly ClienteMapper _mapper;
    private readonly ClienteHandle _sut;

    public ClienteHandlerTests()
    {
        _loggerMock = Substitute.For<ILogger<ClienteHandle>>();
        _clienteService = Substitute.For<IClienteService>();
        _mapper = Substitute.For<ClienteMapper>(Substitute.For<ContaMapper>());
        _sut = new ClienteHandle(_loggerMock, _clienteService, _mapper);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_AdesaoRealizada()
    {
        var request = new AdesaoRequest("", "", "", 1);

        var response = FakerRequest.ClienteAtivo().Generate();
        response.AdicionarConta(ContaGrafica.Gerar(response));
        var result = Result.Success(response);

        _clienteService
            .RealizarAdesaoAsync(Arg.Any<AdesaoRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<AdesaoResponse>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_RealizarAdesaoRetornarErro()
    {
        var request = new AdesaoRequest("", "", "", 1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<Cliente>(exception);

        _clienteService
            .RealizarAdesaoAsync(Arg.Any<AdesaoRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_SaidaProdutoEfetuada()
    {
        var request = new SaidaProdutoRequest(1);

        var response = FakerRequest.ClienteAtivo().Generate();
        var result = Result.Success(response);

        _clienteService
            .SairDoProdutoAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<SaidaProdutoResponse>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_SaidaProdutoRetornarErro()
    {
        var request = new SaidaProdutoRequest(1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<Cliente>(exception);

        _clienteService
            .SairDoProdutoAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_ValorMensalAtualizado()
    {
        var request = new AtualizarValorMensalRequest(1, 100);

        var response = FakerRequest.ClienteAtivo().Generate();
        var result = Result.Success(response);

        _clienteService
            .AtualizarValorMensalAsync(Arg.Any<AtualizarValorMensalRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<AtualizarValorMensalResponse>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_AtualizarValorMensalRetornarErro()
    {
        var request = new AtualizarValorMensalRequest(1, 100);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<Cliente>(exception);

        _clienteService
            .AtualizarValorMensalAsync(Arg.Any<AtualizarValorMensalRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CarteiraConsultada()
    {
        var request = new CarteiraCustodiaRequest(1);

        var response = new CarteiraCustodiaResponse(1, "", "", DateTime.Now, new ResumoCarteiraDto { ValorTotalInvestido = 100, ValorAtualCarteira = 80, PlTotal = 8.4m, RentabilidadePercentual = 0.90m }, new List<DetalheCarteiraDto> { });
        var result = Result.Success(response);

        _clienteService
            .ConsultarCarteiraAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CarteiraCustodiaResponse>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultarCarteiraRetornarErro()
    {
        var request = new CarteiraCustodiaRequest(1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<CarteiraCustodiaResponse>(exception);

        _clienteService
            .ConsultarCarteiraAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_RentabilidadeConsultada()
    {
        var request = new RentabilidadeRequest(1);

        var response = new RentabilidadeResponse(1, "", DateTime.Now, new ResumoCarteiraDto { ValorTotalInvestido = 100, ValorAtualCarteira = 80, PlTotal = 8.4m, RentabilidadePercentual = 0.90m }, new List<HistoricoAporteDto> { }, new List<EvolucaoCarteiraDto> { });
        var result = Result.Success(response);

        _clienteService
            .ConsultarRentabilidadeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<RentabilidadeResponse>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultaRentabilidadeRetornarErro()
    {
        var request = new RentabilidadeRequest(1);

        var exception = new Exception("Erro na compra");
        var result = Result.Error<RentabilidadeResponse>(exception);

        _clienteService
            .ConsultarRentabilidadeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }
}