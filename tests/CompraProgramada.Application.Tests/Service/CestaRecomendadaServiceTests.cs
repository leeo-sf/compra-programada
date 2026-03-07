using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class CestaRecomendadaServiceTests
{
    private readonly Mock<ICestaRecomendadaRepository> _repoMock;
    private readonly CestaRecomendadaService _service;

    public CestaRecomendadaServiceTests()
    {
        _repoMock = new Mock<ICestaRecomendadaRepository>();
        _service = new CestaRecomendadaService(_repoMock.Object);
    }

    [Fact]
    public async Task CriarCestaAsync_DeveRetornarErro_QuandoQuantidadeDiferenteDeCinco()
    {
        var request = new CriarAlterarCestaRequest("Cesta Teste", new List<ComposicaoCestaDto>
        {
            new ComposicaoCestaDto("A", 50),
            new ComposicaoCestaDto("B", 50)
        });

        var result = await _service.CriarCestaAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CriarCestaAsync_DeveRetornarErro_QuandoSomaPercentualDiferenteDe100()
    {
        var request = new CriarAlterarCestaRequest("Cesta Teste", new List<ComposicaoCestaDto>
        {
            new ComposicaoCestaDto("A", 30),
            new ComposicaoCestaDto("B", 30),
            new ComposicaoCestaDto("C", 20),
            new ComposicaoCestaDto("D", 10),
            new ComposicaoCestaDto("E", 5)
        });

        var result = await _service.CriarCestaAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CriarCestaAsync_DeveCriarCestaEDesativarAnterior_QuandoCestaExistente()
    {
        var request = new CriarAlterarCestaRequest("Cesta Teste", new List<ComposicaoCestaDto>
        {
            new ComposicaoCestaDto("A", 20),
            new ComposicaoCestaDto("B", 20),
            new ComposicaoCestaDto("C", 20),
            new ComposicaoCestaDto("D", 20),
            new ComposicaoCestaDto("E", 20)
        });

        var cestaAtual = new CestaRecomendada(1, "Atual", DateTime.UtcNow, DateTime.UtcNow, true) { ComposicaoCesta = new List<ComposicaoCesta> { new ComposicaoCesta(1,1,"A",20) } };
        _repoMock.Setup(r => r.ObterCestaAtivaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(cestaAtual);
        _repoMock.Setup(r => r.AtualizarAsync(It.IsAny<CestaRecomendada>(), It.IsAny<CestaRecomendada>(), It.IsAny<CancellationToken>())).ReturnsAsync((CestaRecomendada a, CestaRecomendada b, CancellationToken ct) => b);
        _repoMock.Setup(r => r.CriarAsync(It.IsAny<CestaRecomendada>(), It.IsAny<CancellationToken>())).ReturnsAsync((CestaRecomendada c, CancellationToken ct) => c with { Id = 2 });

        var result = await _service.CriarCestaAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CestaAtualizada.Should().BeTrue();
        result.Value.CestaAtual.Id.Should().Be(2);
        result.Value.CestaAnterior.Should().NotBeNull();
    }

    [Fact]
    public async Task ObterCestaAtivaAsync_DeveRetornarErro_QuandoNaoExistir()
    {
        _repoMock.Setup(r => r.ObterCestaAtivaAsync(It.IsAny<CancellationToken>())).ReturnsAsync((CestaRecomendada)null);

        var result = await _service.ObterCestaAtivaAsync(CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task HistoricoCestasAsync_DeveRetornarLista()
    {
        var lista = new List<CestaRecomendada>
        {
            new CestaRecomendada(1, "C1", DateTime.UtcNow, DateTime.UtcNow, true) { ComposicaoCesta = new List<ComposicaoCesta>() },
            new CestaRecomendada(2, "C2", DateTime.UtcNow, DateTime.UtcNow, true) { ComposicaoCesta = new List<ComposicaoCesta>() }
        };

        _repoMock.Setup(r => r.ObterTodasCestasAsync(It.IsAny<CancellationToken>())).ReturnsAsync(lista);

        var result = await _service.HistoricoCestasAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public void ObterMudancasDeAtivos_DeveRetornarRemovidosEAdicionados()
    {
        var anterior = new List<ComposicaoCestaRecomendadaDto> { new ComposicaoCestaRecomendadaDto { Ticker = "A" , Percentual = 50, Id = 1, CestaId = 1 }, new ComposicaoCestaRecomendadaDto { Ticker = "B", Percentual = 50, Id = 2, CestaId = 1 } };
        var atual = new List<ComposicaoCestaRecomendadaDto> { new ComposicaoCestaRecomendadaDto { Ticker = "B", Percentual = 60, Id = 2, CestaId = 2 }, new ComposicaoCestaRecomendadaDto { Ticker = "C", Percentual = 40, Id = 3, CestaId = 2 } };

        var result = _service.ObterMudancasDeAtivos(anterior, atual);

        result.IsSuccess.Should().BeTrue();
        result.Value.ativosRemovidos.Should().BeEquivalentTo(new List<string>{ "A" });
        result.Value.ativosAdicionados.Should().BeEquivalentTo(new List<string>{ "C" });
    }

    [Fact]
    public void ValorPorAtivoConsolidado_DeveRetornarValores()
    {
        var cesta = new CestaRecomandadaDto { Id = 1, Nome = "C", DataCriacao = DateTime.UtcNow, Itens = new List<ComposicaoCestaRecomendadaDto> { new ComposicaoCestaRecomendadaDto { Ticker = "A", Percentual = 50, Id = 1, CestaId = 1 }, new ComposicaoCestaRecomendadaDto { Ticker = "B", Percentual = 50, Id = 2, CestaId = 1 } } };

        var result = _service.ValorPorAtivoConsolidado(cesta, 100m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().ContainSingle(v => v.Ticker == "A" && v.ValorDeCompraPorAtivo == 50m);
    }
}
