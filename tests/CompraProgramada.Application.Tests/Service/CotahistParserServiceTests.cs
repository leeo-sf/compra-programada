using CompraProgramada.Application.Config;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using FluentAssertions;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class CotahistParserServiceTests
{
    private readonly AppConfig _appConfig;
    private readonly IFileSystem _fileSystem;
    private readonly CotahistParserService _sut;

    public CotahistParserServiceTests()
    {
        _appConfig = AppConfigHelper.GetAppConfig();
        _fileSystem = Substitute.For<IFileSystem>();
        _sut = new(_appConfig, _fileSystem);
    }

    [Fact]
    public async Task ParseArquivo_Deve_RetornarCotacaoDto_Quando_ArquivoExistir()
    {
        // Arrange
        List<CotacaoB3Dto> resultValue = new()
        {
            new CotacaoB3Dto {
                CodigoBDI = "02",
                DataPregao = DateTime.Parse("2026-03-13"),
                NomeEmpresa = "VITTIA",
                PrecoAbertura = 3.99M,
                PrecoFechamento = 4.01M,
                PrecoMaximo = 4.2M,
                PrecoMedio = 4.04M,
                PrecoMinimo = 3.96M,
                QuantidadeNegociada = 1091700L,
                Ticker = "VITT3",
                TipoMercado = 10,
                VolumeNegociado = 4417843M
            },
            new CotacaoB3Dto
            {
                CodigoBDI = "02",
                DataPregao = DateTime.Parse("2026-03-13"),
                NomeEmpresa = "VIVARA S.A.",
                PrecoAbertura = 26.47M,
                PrecoFechamento = 25M,
                PrecoMaximo = 26.84M,
                PrecoMedio = 24.91M,
                PrecoMinimo = 23.9M,
                QuantidadeNegociada = 16308800L,
                Ticker = "VIVA3",
                TipoMercado = 10,
                VolumeNegociado = 406398617M
            }
        };
        List<string> linhas = new()
        {
            "00COTAHIST.2026BOVESPA 20260313",
            "012026031302VITT3       010VITTIA      ON      NM   R$  000000000039900000000004200000000000396000000000040400000000004010000000000401000000000040704354000000000001091700000000000441784300000000000000009999123100000010000000000000BRVITTACNOR4113",
            "012026031302VIVA3       010VIVARA S.A. ON      NM   R$  000000000264700000000026840000000002390000000000249100000000025000000000002500000000000250649795000000000016308800000000040639861700000000000000009999123100000010000000000000BRVIVAACNOR0109"
        };

        _fileSystem.DiretorioExiste(Arg.Any<string>())
            .Returns(true);

        _fileSystem.ObterArquivo(Arg.Any<string>(), Arg.Any<string>())
            .Returns(new FileInfo("test.txt"));

        _fileSystem.LerLinhas(Arg.Any<string>())
            .Returns(linhas);

        // Act
        var result = _sut.ParseArquivo();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(resultValue);
    }

    [Fact]
    public async Task ParseArquivo_Deve_LancarFileNotFoundException_Quando_ArquivoNaoExistir()
    {
        // Arrange
        List<CotacaoB3Dto> resultValue = new()
        {
            new CotacaoB3Dto {
                CodigoBDI = "02",
                DataPregao = DateTime.Parse("2026-03-13"),
                NomeEmpresa = "VITTIA",
                PrecoAbertura = 3.99M,
                PrecoFechamento = 4.01M,
                PrecoMaximo = 4.2M,
                PrecoMedio = 4.04M,
                PrecoMinimo = 3.96M,
                QuantidadeNegociada = 1091700L,
                Ticker = "VITT3",
                TipoMercado = 10,
                VolumeNegociado = 4417843M
            },
            new CotacaoB3Dto
            {
                CodigoBDI = "02",
                DataPregao = DateTime.Parse("2026-03-13"),
                NomeEmpresa = "VIVARA S.A.",
                PrecoAbertura = 26.47M,
                PrecoFechamento = 25M,
                PrecoMaximo = 26.84M,
                PrecoMedio = 24.91M,
                PrecoMinimo = 23.9M,
                QuantidadeNegociada = 16308800L,
                Ticker = "VIVA3",
                TipoMercado = 10,
                VolumeNegociado = 406398617M
            }
        };
        List<string> linhas = new()
        {
            "00COTAHIST.2026BOVESPA 20260313",
            "012026031302VITT3       010VITTIA      ON      NM   R$  000000000039900000000004200000000000396000000000040400000000004010000000000401000000000040704354000000000001091700000000000441784300000000000000009999123100000010000000000000BRVITTACNOR4113",
            "012026031302VIVA3       010VIVARA S.A. ON      NM   R$  000000000264700000000026840000000002390000000000249100000000025000000000002500000000000250649795000000000016308800000000040639861700000000000000009999123100000010000000000000BRVIVAACNOR0109"
        };

        _fileSystem.DiretorioExiste(Arg.Any<string>())
            .Returns(true);

        _fileSystem.ObterArquivo(Arg.Any<string>(), Arg.Any<string>())
            .Returns((FileInfo)null!);

        // Act
        var act = () => _sut.ParseArquivo();
        var exception = act.Should().Throw<FileNotFoundException>().Which;

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("Nenhum arquivo encontrado na pasta");
    }

    [Fact]
    public async Task ObterCaminhoCompletoArquivoCotacoes_Deve_LancarDirectoryNotFoundException_Quando_DiretorioNaoExistir()
    {
        // Arrange
        _fileSystem.DiretorioExiste(Arg.Any<string>())
            .Returns(false);

        // Act
        var act = () => _sut.ObterCaminhoCompletoArquivoCotacoes();
        var exception = act.Should().Throw<DirectoryNotFoundException>().Which;

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain($"não foi encontrada.");
    }

    [Fact]
    public async Task ObterCaminhoCompletoArquivoCotacoes_Deve_LancarFileNotFoundException_Quando_ArquivoNaoExistir()
    {
        // Arrange
        _fileSystem.DiretorioExiste(Arg.Any<string>())
            .Returns(true);

        _fileSystem.ObterArquivo(Arg.Any<string>(), Arg.Any<string>())
            .Returns((FileInfo)null!);

        // Act
        var act = () => _sut.ObterCaminhoCompletoArquivoCotacoes();
        var exception = act.Should().Throw<FileNotFoundException>().Which;

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("Nenhum arquivo encontrado na pasta");
    }

    [Fact]
    public async Task ObterCaminhoCompletoArquivoCotacoes_Deve_RetornarCaminhoArquivo_Quando_ArquivoExistir()
    {
        // Arrange
        _fileSystem.DiretorioExiste(Arg.Any<string>())
            .Returns(true);

        _fileSystem.ObterArquivo(Arg.Any<string>(), Arg.Any<string>())
            .Returns(new FileInfo("test.txt"));

        // Act
        var result = _sut.ObterCaminhoCompletoArquivoCotacoes();

        // Assert
        result.Should().NotBeEmpty();
    }
}