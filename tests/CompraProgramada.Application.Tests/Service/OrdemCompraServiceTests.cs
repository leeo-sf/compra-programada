using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class OrdemCompraServiceTests
{
    private readonly ILogger<OrdemCompraService> _logger;
    private readonly IOrdemCompraRepository _ordemCompraRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICustodiaMasterService _custodiaMasterService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly OrdemCompraService _sut;

    public OrdemCompraServiceTests()
    {
        _logger = Substitute.For<ILogger<OrdemCompraService>>();
        _ordemCompraRepository = Substitute.For<IOrdemCompraRepository>();
        _cotacaoService = Substitute.For<ICotacaoService>();
        _custodiaMasterService = Substitute.For<ICustodiaMasterService>();
        _cestaRecomendadaService = Substitute.For<ICestaRecomendadaService>();
        _sut = new(_logger, _ordemCompraRepository, _cotacaoService, _custodiaMasterService, _cestaRecomendadaService);
    }

    [Fact]
    public async Task OrdemCompra_Deve_RetornarApplicationExcpetion_Quando_NaoTiverCestaRecomendada()
    {
        // Arrange
        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(Result.Error<CestaRecomendada>(new ApplicationException("Message")));

        // Act
        var result = await _sut.EmitirOrdensDeCompraAsync(1000, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task OrdemCompra_Deve_RetornarException_Quando_FalharObterFechamento()
    {
        // Arrange
        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) }));

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(new ApplicationException("Message"));

        // Act
        var result = await _sut.EmitirOrdensDeCompraAsync(1000, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task OrdemCompra_Deve_RetornarException_Quando_FalharObterResiduos()
    {
        // Arrange
        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) }));

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(Cotacao.CriarRegistro(DateTime.Now, new() { }));

        _custodiaMasterService.ObterResiduosNaoDistribuidos(Arg.Any<CancellationToken>())
            .Returns(new ApplicationException("Message"));

        // Act
        var result = await _sut.EmitirOrdensDeCompraAsync(1000, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task OrdemCompra_Deve_Gerar_Quando_Solicitado_E_SemErros()
    {
        // Arrange
        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) }));

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(Cotacao.CriarRegistro(DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 35), ComposicaoCotacao.CriarItem("VALE3", 62), ComposicaoCotacao.CriarItem("ITUB4", 30), ComposicaoCotacao.CriarItem("BBDC4", 15), ComposicaoCotacao.CriarItem("WEGE3", 40) }));

        _custodiaMasterService.ObterResiduosNaoDistribuidos(Arg.Any<CancellationToken>())
            .Returns(new List<CustodiaMaster>());

        _ordemCompraRepository.SalvarOrdensDeCompra(Arg.Any<List<OrdemCompra>>(), Arg.Any<CancellationToken>())
            .Returns(new List<OrdemCompra>() { OrdemCompra.GerarOrdemCompra("PETR4", 10, 40) });

        // Act
        var result = await _sut.EmitirOrdensDeCompraAsync(1000, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().HaveCount(1);
    }

    [Theory]
    [MemberData(nameof(EmitirOrdensCompra))]
    public void OrdemCompra_Deve_EmitirOrdens_Quando_Solicitado(List<CustodiaMaster> residuos, List<OrdemCompra> ordensCompraGeradas)
    {
        // Arrange
        var totalConsolidado = 3500;
        var cestaVigente = CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) });
        var fechamento = Cotacao.CriarRegistro(DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 35), ComposicaoCotacao.CriarItem("VALE3", 62), ComposicaoCotacao.CriarItem("ITUB4", 30), ComposicaoCotacao.CriarItem("BBDC4", 15), ComposicaoCotacao.CriarItem("WEGE3", 40) });

        // Act
        var result = _sut.EmitirOrdensCompra(fechamento, residuos, cestaVigente, totalConsolidado);

        // Assert
        result.Should().BeEquivalentTo(ordensCompraGeradas, opt =>
            opt.Using<DateTime>(ctx =>
                ctx.Subject.Date.Should().Be(ctx.Expectation.Date))
            .WhenTypeIs<DateTime>());
    }

    public static TheoryData<List<CustodiaMaster>, List<OrdemCompra>> EmitirOrdensCompra()
        => new()
        {
            {
                new() { },
                new()
                {
                    OrdemCompra.GerarOrdemCompra("PETR4", 30, 35),
                    OrdemCompra.GerarOrdemCompra("VALE3", 14, 62),
                    OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30),
                    OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15),
                    OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40),
                }
            },
            {
                new()
                {
                    new CustodiaMaster(0, 0, "PETR4", 2, default!),
                    new CustodiaMaster(0, 0, "VALE3", 0, default!),
                    new CustodiaMaster(0, 0, "ITUB4", 9, default!),
                    new CustodiaMaster(0, 0, "BBDC4", 7, default!),
                    new CustodiaMaster(0, 0, "WEGE3", 0, default!)
                },
                new()
                {
                    OrdemCompra.GerarOrdemCompra("PETR4", 28, 35),
                    OrdemCompra.GerarOrdemCompra("VALE3", 14, 62),
                    OrdemCompra.GerarOrdemCompra("ITUB4", 14, 30),
                    OrdemCompra.GerarOrdemCompra("BBDC4", 28, 15),
                    OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40),
                }
            }
        };
}