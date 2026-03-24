using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class CestaRecomendadaServiceTests
{
    private readonly ILogger<CestaRecomendadaService> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly CestaRecomendadaMapper _mapper;
    private readonly CestaRecomendadaService _sut;

    public CestaRecomendadaServiceTests()
    {
        _logger = Substitute.For<ILogger<CestaRecomendadaService>>();
        _cestaRecomendadaRepository = Substitute.For<ICestaRecomendadaRepository>();
        _mapper = Substitute.For<CestaRecomendadaMapper>();
        _sut = new(_logger, _cestaRecomendadaRepository, _mapper);
    }

    [Fact]
    public async Task CestaRecomendadaService_Deve_CriarNovaCesta_Quando_NaoTiverUmaCestaAtiva()
    {
        // Arrange
        var request = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaCriada = CestaRecomendada.CriarCesta(request.Nome, request.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        _cestaRecomendadaRepository.CriarAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(cestaCriada);

        // Act
        var result = await _sut.CriarCestaAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.CestaAtualizada.Should().BeFalse();
        result.Value!.CestaAtual.Should().NotBeNull();
        result.Value!.CestaAnterior.Should().BeNull();
    }

    [Fact]
    public async Task CestaRecomendadaService_Deve_CriarNovaCesta_E_DesativarAnterior_Quando_TiverUmaCestaAtiva()
    {
        // Arrange
        var request = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaCriada = CestaRecomendada.CriarCesta(request.Nome, request.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        var itensCestaAnterior = new List<ComposicaoCestaDto> { new("AAPL4", 30), new("VALE3", 25), new("ITUB4", 20), new("TEST5", 10), new("WEGE3", 15) };
        var cestaAnterior = CestaRecomendada.CriarCesta("Cesta Top Five Fast", itensCestaAnterior.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAnterior);

        _cestaRecomendadaRepository.CriarAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(cestaCriada);

        // Act
        var result = await _sut.CriarCestaAsync(request, CancellationToken.None);

        var cestaRecomendadaResult = result.Value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        cestaRecomendadaResult.Should().NotBeNull();
        cestaRecomendadaResult.CestaAtualizada.Should().BeTrue();
        cestaRecomendadaResult.CestaAtual.Should().NotBeNull();
        cestaRecomendadaResult.CestaAtual.Nome.Should().Be(cestaCriada.Nome);
        cestaRecomendadaResult.CestaAtual.Ativa.Should().BeTrue();
        cestaRecomendadaResult.CestaAnterior.Should().NotBeNull();
        cestaRecomendadaResult.CestaAnterior.Nome.Should().Be(cestaAnterior.Nome);
        cestaRecomendadaResult.CestaAnterior.Ativa.Should().BeFalse();
    }

    [Fact]
    public async Task CestaRecomendadaService_Deve_RetornarCesta_Quando_TiverCestaAtiva()
    {
        // Arrange
        var responseCestaAtiva = CestaRecomendada.CriarCesta("Cesta", FakerRequest.ComposicaoCestaRecomendada().Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(responseCestaAtiva);

        // Act
        var result = await _sut.ObterCestaAtivaAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Ativa.Should().BeTrue();
        result.Value.Nome.Should().Be("CESTA");
    }

    [Fact]
    public async Task CestaRecomendadaService_Deve_RetornarApplicationException_Quando_NaoTiverCestaAtiva()
    {
        // Arrange
        var exception = new ApplicationException("Nenhuma Cesta Top Five ativa no momento.");

        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        // Act
        var result = await _sut.ObterCestaAtivaAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Nenhuma Cesta Top Five ativa no momento.");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task CestaRecomendadaService_Deve_RetornarHistoricoDeCesta_Quando_TiverCestasAtiva()
    {
        // Arrange
        var response = new List<CestaRecomendada> { CestaRecomendada.CriarCesta("Cesta", FakerRequest.ComposicaoCestaRecomendada().Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList()), CestaRecomendada.CriarCesta("Cesta Dois", FakerRequest.ComposicaoCestaRecomendada().Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList()) };

        _cestaRecomendadaRepository.ObterTodasCestasAsync(Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.HistoricoCestasAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<List<CestaRecomendada>>();
    }

    [Fact]
    public async Task CestaRecomendadaService_Deve_RetornarHistoricoVazio_Quando_NaoTiverCestasAtiva()
    {
        _cestaRecomendadaRepository.ObterTodasCestasAsync(Arg.Any<CancellationToken>())
            .Returns((List<CestaRecomendada>)null!);

        // Act
        var result = await _sut.HistoricoCestasAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(ValorPorAtivoConsolidadoRequest))]
    public async Task CestaRecomendadaService_Deve_RetornarValorPorAtivoConsolidado_Quando_TiverCestaAtual(CestaRecomendada cestaAtual, decimal totalAtivoConsolidado, List<ValorAtivoConsolidadoDto> resultadoEsperado)
    {
        // Arrange
        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtual);

        // Arrange & Act
        var valorPorAtivoConsolidadoResult = await _sut.ValorPorAtivoConsolidado(totalAtivoConsolidado, CancellationToken.None);

        // Assert
        valorPorAtivoConsolidadoResult.IsSuccess.Should().BeTrue();
        valorPorAtivoConsolidadoResult.Exception.Should().BeNull();
        valorPorAtivoConsolidadoResult.Value.Should().NotBeNull();
        valorPorAtivoConsolidadoResult.Value.Should().BeEqualTo(resultadoEsperado);
    }

    [Theory]
    [MemberData(nameof(MudancaAtivosRequest))]
    public void CestaRecomendadaService_Deve_RetornarAtivosAdicionados_E_Removidos_Quando_HouverAlteracao(List<ComposicaoCestaRecomendadaDto> composicaoAnterior, List<string> ativosRemovidos, List<ComposicaoCestaRecomendadaDto> composicaoAtual, List<string> ativosAdicionados)
    {
        // Arrange & Act
        var (ativosRemovidosResult, ativosAdicionadosResult) = _sut.ObterMudancasDeAtivos(composicaoAnterior, composicaoAtual);

        // Assert
        ativosRemovidosResult.Should().NotBeEmpty();
        ativosAdicionadosResult.Should().NotBeEmpty();
        ativosRemovidosResult.Should().BeEqualTo(ativosRemovidos);
        ativosAdicionadosResult.Should().BeEqualTo(ativosAdicionados);
    }

    public static TheoryData<CestaRecomendada, decimal, List<ValorAtivoConsolidadoDto>> ValorPorAtivoConsolidadoRequest()
    {
        return new()
        {
            {
                CestaRecomendada.CriarCesta("Cesta", FakerRequest.ComposicaoCestaRecomendada().Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList()),
                3500,
                new List<ValorAtivoConsolidadoDto> { new("PETR4", 1050), new("VALE3", 875), new("ITUB4", 700), new("BBDC4", 525), new("WEGE3", 350) }
            },
            {
                CestaRecomendada.CriarCesta("Cesta", new List<ComposicaoCesta> { ComposicaoCesta.CriaItemNaCesta("PETR4", 21), ComposicaoCesta.CriaItemNaCesta("VALE3", 12), ComposicaoCesta.CriaItemNaCesta("AAPL4", 47), ComposicaoCesta.CriaItemNaCesta("BBDC4", 6), ComposicaoCesta.CriaItemNaCesta("WEGE3", 14) }),
                22987.90m,
                new List<ValorAtivoConsolidadoDto> { new("PETR4", 4827.4590m), new("VALE3", 2758.548m), new("AAPL4", 10804.313m), new("BBDC4", 1379.274m), new("WEGE3", 3218.306m) }
            }
        };
    }

    public static TheoryData<List<ComposicaoCestaRecomendadaDto>, List<string>, List<ComposicaoCestaRecomendadaDto>, List<string>> MudancaAtivosRequest()
    {
        return new()
        {
            {
                new List<ComposicaoCestaRecomendadaDto> { new(0, 1, "PETR4", 30), new(0, 1, "AAPL4", 15), new(0, 1, "ITUB4", 25) },
                new List<string> { "AAPL4", "ITUB4" },
                new List<ComposicaoCestaRecomendadaDto> { new(0, 1, "PETR4", 30), new(0, 1, "TEST5", 15), new(0, 1, "WEGE3", 25) },
                new List<string> { "TEST5", "WEGE3" }
            },
            {
                new List<ComposicaoCestaRecomendadaDto> { new(0, 1, "PETR4", 30), new(0, 1, "VALE3", 15), new(0, 1, "RENT3", 25) },
                new List<string> { "VALE3" },
                new List<ComposicaoCestaRecomendadaDto> { new(0, 1, "PETR4", 35), new(0, 1, "ABEV3", 17), new(0, 1, "RENT3", 10) },
                new List<string> { "ABEV3" }
            }
        };
    }
}