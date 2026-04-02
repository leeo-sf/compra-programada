using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Tests.Service;

public class CotacaoServiceTests
{
    private readonly ILogger<CotacaoService> _logger;
    private readonly ICotacaoRepository _cotacaoRepository;
    private readonly ICotahistParserService _cotahistParserService;
    private readonly CotacaoService _sut;

    public CotacaoServiceTests()
    {
        _logger = Substitute.For<ILogger<CotacaoService>>();
        _cotacaoRepository = Substitute.For<ICotacaoRepository>();
        _cotahistParserService = Substitute.For<ICotahistParserService>();
        _sut = new(_logger, _cotacaoRepository, _cotahistParserService);
    }

    [Fact]
    public async Task CotacaoService_Deve_RetornarApplicationException_Quando_ItensCestaVazio()
    {
        // Arrange
        var cestaVigente = CestaRecomendada.CriarCesta("Cesta", new() { ComposicaoCesta.CriaItemNaCesta("TEST1", 30), ComposicaoCesta.CriaItemNaCesta("TEST2", 25), ComposicaoCesta.CriaItemNaCesta("TEST3", 20), ComposicaoCesta.CriaItemNaCesta("TEST4", 15), ComposicaoCesta.CriaItemNaCesta("TEST5", 10) });

        _cotahistParserService.ParseArquivo()
            .Returns(CotcaoesB3());

        // Act
        var result = await _sut.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Não foi possível obter a cesta recomendada nas cotações da B3.");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task CotacaoService_Deve_Salvar_E_RetornarCotacao_Quando_ItensCesta_Existir()
    {
        // Arrange
        var resultValue = Cotacao.CriarRegistro(
            DateTime.MinValue,
            new()
            {
                ComposicaoCotacao.CriarItem("PETR4", 35),
                ComposicaoCotacao.CriarItem("VALE3", 62),
                ComposicaoCotacao.CriarItem("ITUB4", 30),
                ComposicaoCotacao.CriarItem("BBDC4", 15),
                ComposicaoCotacao.CriarItem("WEGE3", 40)
            }
        );
        var cestaVigente = CestaRecomendada.CriarCesta("Cesta", FakerRequest.CriarCestaRecomendadaRequest().Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _cotahistParserService.ParseArquivo()
            .Returns(CotcaoesB3());

        _cotacaoRepository.SalvarCotacaoAsync(Arg.Any<Cotacao>(), Arg.Any<CancellationToken>())
            .Returns(resultValue);

        // Act
        var result = await _sut.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void CotacaoService_Deve_RealizarMatch_Fechamento_X_CestaRecomendada_Quando_Solicitado()
    {
        // Arrange
        var cestaVigente = CestaRecomendada.CriarCesta("Cesta", FakerRequest.CriarCestaRecomendadaRequest().Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());
        IEnumerable<CotacaoB3Dto> resultValue = new List<CotacaoB3Dto>()
        {
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "WEGE3",
                TipoMercado = 0,
                PrecoAbertura = 60,
                PrecoMaximo = 62,
                PrecoMinimo = 40,
                PrecoFechamento = 40,
                PrecoMedio = 57,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "ITUB4",
                TipoMercado = 0,
                PrecoAbertura = 22,
                PrecoMaximo = 33,
                PrecoMinimo = 21,
                PrecoFechamento = 30,
                PrecoMedio = 30,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "PETR4",
                TipoMercado = 0,
                PrecoAbertura = 30,
                PrecoMaximo = 37,
                PrecoMinimo = 31,
                PrecoFechamento = 35,
                PrecoMedio = 33,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "VALE3",
                TipoMercado = 0,
                PrecoAbertura = 60,
                PrecoMaximo = 69,
                PrecoMinimo = 60,
                PrecoFechamento = 62,
                PrecoMedio = 64,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "BBDC4",
                TipoMercado = 0,
                PrecoAbertura = 15,
                PrecoMaximo = 18,
                PrecoMinimo = 12,
                PrecoFechamento = 15,
                PrecoMedio = 15,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            }
        };

        _cotahistParserService.ParseArquivo()
            .Returns(CotcaoesB3());

        // Act
        var result = _sut.RealizarMatchFechamentoECestaRecomendada(cestaVigente);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(5);
        result.Should().BeEquivalentTo(resultValue);
    }

    [Fact]
    public void CotacaoService_Deve_RetornarNul_Quando_RealizarMatch_Fechamento_X_CestaRecomendada_E_NaoExistir_Ticker()
    {
        // Arrange
        var cestaVigente = CestaRecomendada.CriarCesta("Cesta", new() { ComposicaoCesta.CriaItemNaCesta("TEST1", 30), ComposicaoCesta.CriaItemNaCesta("TEST2", 25), ComposicaoCesta.CriaItemNaCesta("TEST3", 20), ComposicaoCesta.CriaItemNaCesta("TEST4", 15), ComposicaoCesta.CriaItemNaCesta("TEST5", 10) });

        _cotahistParserService.ParseArquivo()
            .Returns(CotcaoesB3());

        // Act
        var result = _sut.RealizarMatchFechamentoECestaRecomendada(cestaVigente);

        // Assert
        result.Should().BeEmpty();
    }

    public static IEnumerable<CotacaoB3Dto> CotcaoesB3()
        => new List<CotacaoB3Dto>
        {
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "BBAS3",
                TipoMercado = 0,
                PrecoAbertura = 32,
                PrecoMaximo = 38,
                PrecoMinimo = 30,
                PrecoFechamento = 35,
                PrecoMedio = 34,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "WEGE3",
                TipoMercado = 0,
                PrecoAbertura = 60,
                PrecoMaximo = 62,
                PrecoMinimo = 40,
                PrecoFechamento = 40,
                PrecoMedio = 57,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "AAPL4",
                TipoMercado = 0,
                PrecoAbertura = 63,
                PrecoMaximo = 69,
                PrecoMinimo = 61,
                PrecoFechamento = 67.56m,
                PrecoMedio = 60,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "ITUB4",
                TipoMercado = 0,
                PrecoAbertura = 22,
                PrecoMaximo = 33,
                PrecoMinimo = 21,
                PrecoFechamento = 30,
                PrecoMedio = 30,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "ABEV3",
                TipoMercado = 0,
                PrecoAbertura = 47,
                PrecoMaximo = 58,
                PrecoMinimo = 47,
                PrecoFechamento = 55.13m,
                PrecoMedio = 55,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "PETR4",
                TipoMercado = 0,
                PrecoAbertura = 30,
                PrecoMaximo = 37,
                PrecoMinimo = 31,
                PrecoFechamento = 35,
                PrecoMedio = 33,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "VALE3",
                TipoMercado = 0,
                PrecoAbertura = 60,
                PrecoMaximo = 69,
                PrecoMinimo = 60,
                PrecoFechamento = 62,
                PrecoMedio = 64,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            },
            new CotacaoB3Dto
            {
                DataPregao = DateTime.MinValue,
                Ticker = "BBDC4",
                TipoMercado = 0,
                PrecoAbertura = 15,
                PrecoMaximo = 18,
                PrecoMinimo = 12,
                PrecoFechamento = 15,
                PrecoMedio = 15,
                QuantidadeNegociada = 1000,
                VolumeNegociado = 35000,
            }
        };
}