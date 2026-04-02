using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using CompraProgramada.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OperationResult;

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

        var itensCestaAnterior = new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "AAPL4", Percentual = 30 }, new ComposicaoCestaDto { Ticker = "VALE3", Percentual = 25 }, new ComposicaoCestaDto { Ticker = "ITUB4", Percentual = 20 }, new ComposicaoCestaDto { Ticker = "TEST5", Percentual = 10 }, new ComposicaoCestaDto { Ticker = "WEGE3", Percentual = 15 } };
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

    [Fact]
    public async Task ValorPorAtivoConsolidado_Deve_RetornarException_Quando_CestaVigenteFalhar()
    {
        // Arrange
        var exception = new ApplicationException("Nenhuma Cesta Top Five ativa no momento.");

        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        // Act
        var result = await _sut.ValorPorAtivoConsolidado(100, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Nenhuma Cesta Top Five ativa no momento.");
        result.Value.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(ValorPorAtivoConsolidadoRequest))]
    public async Task ValorPorAtivoConsolidado_Deve_RetornarValorPorAtivoConsolidado_Quando_TiverCestaAtual(CestaRecomendada cestaAtual, decimal totalAtivoConsolidado, List<ValorAtivoConsolidadoDto> resultadoEsperado)
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
        valorPorAtivoConsolidadoResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Theory]
    [MemberData(nameof(MudancaAtivosRequest))]
    public void MudancaAtivos_Deve_RetornarAtivosAdicionados_E_Removidos_Quando_HouverAlteracao(List<ComposicaoCestaDto> composicaoAnterior, List<string> ativosRemovidos, List<ComposicaoCestaDto> composicaoAtual, List<string> ativosAdicionados)
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
                new List<ValorAtivoConsolidadoDto> { new ValorAtivoConsolidadoDto { Ticker = "PETR4", ValorDeCompraAtivo = 1050.0m }, new ValorAtivoConsolidadoDto { Ticker = "VALE3", ValorDeCompraAtivo = 875.00m }, new ValorAtivoConsolidadoDto { Ticker = "ITUB4", ValorDeCompraAtivo = 700.0m }, new ValorAtivoConsolidadoDto { Ticker = "BBDC4", ValorDeCompraAtivo = 525.00m }, new ValorAtivoConsolidadoDto { Ticker = "WEGE3", ValorDeCompraAtivo = 350.0m } }
            },
            {
                CestaRecomendada.CriarCesta("Cesta", new List<ComposicaoCesta> { ComposicaoCesta.CriaItemNaCesta("PETR4", 21), ComposicaoCesta.CriaItemNaCesta("VALE3", 12), ComposicaoCesta.CriaItemNaCesta("AAPL4", 47), ComposicaoCesta.CriaItemNaCesta("BBDC4", 6), ComposicaoCesta.CriaItemNaCesta("WEGE3", 14) }),
                22987.90m,
                new List<ValorAtivoConsolidadoDto> { new ValorAtivoConsolidadoDto { Ticker = "PETR4", ValorDeCompraAtivo = 4827.4590m }, new ValorAtivoConsolidadoDto { Ticker = "VALE3", ValorDeCompraAtivo = 2758.548m }, new ValorAtivoConsolidadoDto { Ticker = "AAPL4", ValorDeCompraAtivo = 10804.313m }, new ValorAtivoConsolidadoDto { Ticker = "BBDC4", ValorDeCompraAtivo = 1379.274m }, new ValorAtivoConsolidadoDto { Ticker = "WEGE3", ValorDeCompraAtivo = 3218.306m } }
            }
        };
    }

    public static TheoryData<List<ComposicaoCestaDto>, List<string>, List<ComposicaoCestaDto>, List<string>> MudancaAtivosRequest()
    {
        return new()
        {
            {
                new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "PETR4", Percentual = 30 }, new ComposicaoCestaDto { Ticker = "AAPL4", Percentual = 15 }, new ComposicaoCestaDto { Ticker = "ITUB4", Percentual = 25 } },
                new List<string> { "AAPL4", "ITUB4" },
                new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "PETR4", Percentual = 30 }, new ComposicaoCestaDto { Ticker = "TEST5", Percentual = 15 }, new ComposicaoCestaDto { Ticker = "WEGE3", Percentual = 25 } },
                new List<string> { "TEST5", "WEGE3" }
            },
            {
                new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "PETR4", Percentual = 30 }, new ComposicaoCestaDto { Ticker = "VALE3", Percentual = 15 }, new ComposicaoCestaDto { Ticker = "RENT3", Percentual = 25 } },
                new List<string> { "VALE3" },
                new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "PETR4", Percentual = 35 }, new ComposicaoCestaDto { Ticker = "ABEV3", Percentual = 17 }, new ComposicaoCestaDto { Ticker = "RENT3", Percentual = 10 } },
                new List<string> { "ABEV3" }
            }
        };
    }
}