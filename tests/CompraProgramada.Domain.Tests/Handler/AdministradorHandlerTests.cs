using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Domain.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using OperationResult;
using CompraProgramada.Domain.Handler;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Mapper;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Handler;

public class AdministradorHandlerTests
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly IClienteService _clienteService;
    private readonly CestaRecomendadaMapper _mapper;
    private readonly AdministradorHandler _sut;

    public AdministradorHandlerTests()
    {
        _logger = Substitute.For<ILogger<AdministradorHandler>>();
        _cestaRecomendadaService = Substitute.For<ICestaRecomendadaService>();
        _clienteService = Substitute.For<IClienteService>();
        _mapper = Substitute.For<CestaRecomendadaMapper>();
        _sut = new AdministradorHandler(_logger, _clienteService, _cestaRecomendadaService, _mapper);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CriarCestaRealizada()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var response = new CriarCestaRecomendadaDto { CestaAtualizada = false, CestaAtual = new CestaRecomendadaDto { CestaId = 1, Nome = "", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new List<ComposicaoCestaDto> { } }, CestaAnterior = null };
        var result = Result.Success(response);

        _cestaRecomendadaService
            .CriarCestaAsync(Arg.Any<CriarCestaRecomendadaRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CriarCestaRecomendadaResponse>();
        resultado.Value.CestaAnteriorDesativada.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CriarCestaAtualizada()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var response = new CriarCestaRecomendadaDto { CestaAtualizada = true, CestaAtual = new CestaRecomendadaDto { CestaId = 2, Nome = "Cesta Test 2", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new List<ComposicaoCestaDto> { } }, CestaAnterior = new CestaRecomendadaDto { CestaId = 1, Nome = "Cesta Test", DataCriacao = DateTime.MinValue, DataDesativacao = DateTime.Now, Ativa = false, Itens = new List<ComposicaoCestaDto> { } } };
        var result = Result.Success(response);

        _cestaRecomendadaService
            .CriarCestaAsync(Arg.Any<CriarCestaRecomendadaRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        _clienteService
            .QuantidadeClientesAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(0);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CriarCestaRecomendadaResponse>();
        resultado.Value.CestaAnteriorDesativada.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_CriarCestaFalhar()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var exception = new Exception("Erro na compra");
        var result = Result.Error<CriarCestaRecomendadaDto>(exception);

        _cestaRecomendadaService
            .CriarCestaAsync(Arg.Any<CriarCestaRecomendadaRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_QuantidadeClientesAtivos_Falhar()
    {
        var request = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 10 } });

        var response = new CriarCestaRecomendadaDto { CestaAtualizada = true, CestaAtual = new CestaRecomendadaDto { CestaId = 2, Nome = "Cesta Test 2", DataCriacao = DateTime.Now, DataDesativacao = null, Ativa = true, Itens = new List<ComposicaoCestaDto> { } }, CestaAnterior = new CestaRecomendadaDto { CestaId = 1, Nome = "Cesta Test", DataCriacao = DateTime.MinValue, DataDesativacao = DateTime.Now, Ativa = false, Itens = new List<ComposicaoCestaDto> { } } };
        var result = Result.Success(response);

        _cestaRecomendadaService.CriarCestaAsync(Arg.Any<CriarCestaRecomendadaRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);

        _clienteService.QuantidadeClientesAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(new Exception("Error"));

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeOfType<Exception>();
        resultado.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CestaAtualConsultada()
    {
        var request = new CestaAtualRequest();

        var itensCesta = FakerRequest.ComposicaoCestaRecomendada();
        var response = CestaRecomendada.CriarCesta("Name", itensCesta.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());
        var result = Result.Success(response);

        _cestaRecomendadaService
            .ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(Result.Success(response));

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<CestaRecomendadaDto>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultarCestaAtualFalhar()
    {
        var request = new CestaAtualRequest();

        var exception = new Exception("Erro na compra");
        var result = Result.Error<CestaRecomendada>(exception);

        _cestaRecomendadaService
            .ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_HistoricoCestaConsultada()
    {
        var request = new CestaHistoricoRequest();

        var response = new List<CestaRecomendada> { };

        _cestaRecomendadaService.HistoricoCestasAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<HistoricoCestasResponse>();
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_Quando_ConsultarHistoricoCestaFalhar()
    {
        var request = new CestaHistoricoRequest();

        var exception = new Exception("Erro na compra");
        var result = Result.Error<List<CestaRecomendada>>(exception);

        _cestaRecomendadaService
            .HistoricoCestasAsync(Arg.Any<CancellationToken>())!
            .Returns(result);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
    }
}