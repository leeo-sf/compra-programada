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

namespace CompraProgramada.Domain.Tests.Handler;

public class AdministradorHandlerTests
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly CestaRecomendadaMapper _mapper;
    private readonly AdministradorHandler _sut;

    public AdministradorHandlerTests()
    {
        _logger = Substitute.For<ILogger<AdministradorHandler>>();
        _cestaRecomendadaRepository = Substitute.For<ICestaRecomendadaRepository>();
        _clienteRepository = Substitute.For<IClienteRepository>();
        _mapper = Substitute.For<CestaRecomendadaMapper>();
        _sut = new AdministradorHandler(_logger, _cestaRecomendadaRepository, _clienteRepository, _mapper);
    }

    [Fact]
    public async Task Handle_Deve_CriarCesta_Quando_NaoTiverUmaCestaAtiva()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        _cestaRecomendadaRepository
            .CriarAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(new CestaRecomendada(0, "", DateTime.MinValue, DateTime.MinValue, true, []));

        var result = await _sut.Handle(request, CancellationToken.None);

        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        response.Should().BeOfType<CriarCestaRecomendadaResponse>();
        response.CestaAnteriorDesativada.Should().BeNull();
        response.RebalanceamentoDisparado.Should().BeFalse();
        response.Mensagem.Should().Be("Primeira cesta cadastrada com sucesso.");
    }

    [Fact]
    public async Task Handle_Deve_CriarNovaCesta_E_DesativarAnterior_Quando_TiverUmaCestaAtiva()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAnterior = new CestaRecomendada(1, "Cesta Anterior", DateTime.MinValue, DateTime.MinValue, true, []);

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAnterior);

        _cestaRecomendadaRepository
            .CriarAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(new CestaRecomendada(0, "", DateTime.MinValue, DateTime.MinValue, true, []));

        _clienteRepository.QuantidadeAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _sut.Handle(request, CancellationToken.None);

        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        response.Should().BeOfType<CriarCestaRecomendadaResponse>();
        response.CestaAnteriorDesativada.Should().NotBeNull();
        response.RebalanceamentoDisparado.Should().BeTrue();
        response.Mensagem.Should().Be("Cesta atualizada. Rebalanceamento disparado para 1 clientes ativos.");
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CestaAtualConsultada_ComSucesso()
    {
        var request = new CestaAtualRequest();

        var itensCesta = FakerRequest.ComposicaoCestaRecomendada();
        var response = CestaRecomendada.CriarCesta("Name", [.. itensCesta.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual))]);

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())!
            .Returns(response);

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

    [Theory]
    [MemberData(nameof(MudancaAtivosRequest))]
    public void MudancaAtivos_Deve_RetornarAtivosAdicionados_E_Removidos_Quando_HouverAlteracao(List<ComposicaoCesta> composicaoAnterior, List<string> ativosRemovidos, List<ComposicaoCesta> composicaoAtual, List<string> ativosAdicionados)
    {
        // Arrange & Act
        var (ativosRemovidosResult, ativosAdicionadosResult) = _sut.ObterMudancasDeAtivos(composicaoAnterior, composicaoAtual);

        // Assert
        ativosRemovidosResult.Should().NotBeEmpty();
        ativosAdicionadosResult.Should().NotBeEmpty();
        ativosRemovidosResult.Should().BeEqualTo(ativosRemovidos);
        ativosAdicionadosResult.Should().BeEqualTo(ativosAdicionados);
    }

    public static TheoryData<List<ComposicaoCesta>, List<string>, List<ComposicaoCesta>, List<string>> MudancaAtivosRequest()
    {
        return new()
        {
            {
                new List<ComposicaoCesta> { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("AAPL4", 15), ComposicaoCesta.CriaItemNaCesta("ITUB4", 25) },
                new List<string> { "AAPL4", "ITUB4" },
                new List<ComposicaoCesta> { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("TEST5", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 25) },
                new List<string> { "TEST5", "WEGE3" }
            },
            {
                new List<ComposicaoCesta> { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 15), ComposicaoCesta.CriaItemNaCesta("RENT3", 25) },
                new List<string> { "VALE3" },
                new List<ComposicaoCesta> { ComposicaoCesta.CriaItemNaCesta("PETR4", 35), ComposicaoCesta.CriaItemNaCesta("ABEV3", 17), ComposicaoCesta.CriaItemNaCesta("RENT3", 10) },
                new List<string> { "ABEV3" }
            }
        };
    }
}