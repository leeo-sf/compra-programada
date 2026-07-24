using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Domain.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using CompraProgramada.Domain.Mapper;
using NSubstitute;
using CompraProgramada.Domain.Handler.Api;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using OperationResult;

namespace CompraProgramada.Domain.Tests.Handler;

public class ConsultarCestaHandlerTests
{
    private readonly ILogger<ConsultarCestaHandler> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly CestaRecomendadaMapper _mapper;
    private readonly ConsultarCestaHandler _sut;

    public ConsultarCestaHandlerTests()
    {
        _logger = Substitute.For<ILogger<ConsultarCestaHandler>>();
        _cestaRecomendadaRepository = Substitute.For<ICestaRecomendadaRepository>();
        _cotacaoService = Substitute.For<ICotacaoService>();
        _mapper = Substitute.For<CestaRecomendadaMapper>();
        _sut = new(_logger, _cestaRecomendadaRepository, _cotacaoService, _mapper);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CestaAtualConsultada_ComSucesso()
    {
        var request = new CestaAtualRequest();

        var itensCesta = FakerRequest.ComposicaoCestaRecomendada();
        var cesta = CestaRecomendada.CriarCesta("Name", [.. itensCesta.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual))]);

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())!
            .Returns(cesta);

        _cotacaoService
            .ObterCotacoesDaCestaRecomendadaAsync(cesta, Arg.Any<CancellationToken>())
            .Returns(Cotacao.CriarRegistro(DateOnly.MinValue, []));

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
        resultado.Value.Should().BeOfType<CestaRecomendadaDto>();
    }

    [Fact]
    public async Task Handle_DeveRetornarApplicationException_Quando_NaoTiverCestaAtiva()
    {
        var request = new CestaAtualRequest();

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())!
            .Returns((CestaRecomendada)null!);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeOfType<ApplicationException>();
        resultado.Exception.Message.Should().Be("Nenhuma Cesta Top Five ativa no momento.");
    }

    [Fact]
    public async Task Handle_DeveRetornarException_Quando_ObterCotacaoAtualFalhar()
    {
        var request = new CestaAtualRequest();

        var itensCesta = FakerRequest.ComposicaoCestaRecomendada();
        var response = CestaRecomendada.CriarCesta("Name", [.. itensCesta.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual))]);

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())!
            .Returns(response);

        _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error<Cotacao>(new Exception("Erro ao obter cotações.")));

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeOfType<Exception>();
        resultado.Exception.Message.Should().Be("Erro ao obter cotações.");
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_HistoricoCestaConsultada()
    {
        var request = new CestaHistoricoRequest();
        var response = new List<CestaRecomendada> { };

        _cestaRecomendadaRepository.ObterCestasAsync(Arg.Any<CancellationToken>())
            .Returns(response);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
        resultado.Value.Should().BeOfType<HistoricoCestasResponse>();
    }
}