using CompraProgramada.Application.Service;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class CotacaoServiceTests
{
    private readonly Mock<ICotacaoRepository> _cotacaoRepoMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<ICotahistParserService> _parserMock;
    private readonly Mock<ICestaRecomendadaService> _cestaMock;
    private readonly CotacaoService _service;

    public CotacaoServiceTests()
    {
        _cotacaoRepoMock = new Mock<ICotacaoRepository>();
        _fileServiceMock = new Mock<IFileService>();
        _parserMock = new Mock<ICotahistParserService>();
        _cestaMock = new Mock<ICestaRecomendadaService>();

        var logger = NullLogger<CotacaoService>.Instance;
        _service = new CotacaoService(logger, _cotacaoRepoMock.Object, _fileServiceMock.Object, _parserMock.Object, _cestaMock.Object);
    }

    [Fact]
    public async Task ObterCotacaoAsync_DeveRetornarErro_QuandoNaoExistir()
    {
        _cotacaoRepoMock.Setup(r => r.ObterCotacaoAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cotacao)null);

        var result = await _service.ObterCotacaoAsync(DateTime.UtcNow, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ObterCotacoesFechamentoB3DaCestaRecomendadaAsync_DeveLancar_QuandoNaoExistirCesta()
    {
        _fileServiceMock.Setup(f => f.ObterCaminhoCompletoArquivoCotacoes()).Returns("path");
        _parserMock.Setup(p => p.ParseArquivo(It.IsAny<string>())).Returns(new List<CotacaoB3Dto>());
        _cestaMock.Setup(c => c.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Error<CestaRecomandadaDto>(new Exception("nenhuma")));

        await Assert.ThrowsAsync<ApplicationException>(async () => await _service.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CancellationToken.None));
    }

    [Fact]
    public async Task ObterCotacoesFechamentoB3DaCestaRecomendadaAsync_DeveLancar_QuandoCotacoesNaoContemCesta()
    {
        _fileServiceMock.Setup(f => f.ObterCaminhoCompletoArquivoCotacoes()).Returns("path");
        _parserMock.Setup(p => p.ParseArquivo(It.IsAny<string>())).Returns(new List<CotacaoB3Dto> { new CotacaoB3Dto { Ticker = "AAA", PrecoFechamento = 10m, DataPregao = DateTime.UtcNow } });
        var cesta = new CestaRecomandadaDto { Id = 1, Nome = "C1", DataCriacao = DateTime.UtcNow, Itens = new List<ComposicaoCestaRecomendadaDto> { new ComposicaoCestaRecomendadaDto { Ticker = "BBB", Percentual = 50, Id = 1, CestaId = 1 } } };
        _cestaMock.Setup(c => c.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Success(cesta));

        await Assert.ThrowsAsync<ApplicationException>(async () => await _service.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CancellationToken.None));
    }

    [Fact]
    public async Task ObterCotacoesFechamentoB3DaCestaRecomendadaAsync_DeveRetornarCotacoesDaCesta()
    {
        _fileServiceMock.Setup(f => f.ObterCaminhoCompletoArquivoCotacoes()).Returns("path");
        var cotacaoB3 = new CotacaoB3Dto { Ticker = "PETR4", PrecoFechamento = 10m, DataPregao = DateTime.UtcNow };
        _parserMock.Setup(p => p.ParseArquivo(It.IsAny<string>())).Returns(new List<CotacaoB3Dto> { cotacaoB3 });
        var cesta = new CestaRecomandadaDto { Id = 1, Nome = "C1", DataCriacao = DateTime.UtcNow, Itens = new List<ComposicaoCestaRecomendadaDto> { new ComposicaoCestaRecomendadaDto { Ticker = "PETR4", Percentual = 50, Id = 1, CestaId = 1 } } };
        _cestaMock.Setup(c => c.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Success(cesta));

        _cotacaoRepoMock.Setup(r => r.SalvarCotacaoAsync(It.IsAny<Cotacao>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cotacao c, CancellationToken ct) => c with { Id = 1 });

        var result = await _service.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task SalvarCotacaoAsync_DeveSalvarECopiarItens()
    {
        var dto = new CotacaoDto { DataPregao = DateTime.UtcNow, Itens = new List<ComposicaoCotacaoDto> { new ComposicaoCotacaoDto("TICK", 10m) } };
        _cotacaoRepoMock.Setup(r => r.SalvarCotacaoAsync(It.IsAny<Cotacao>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cotacao c, CancellationToken ct) => c with { Id = 2 });

        var result = await _service.SalvarCotacaoAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Itens.Should().HaveCount(1);
    }
}