using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Service;
using CompraProgramada.Shared.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OperationResult;

namespace CompraProgramada.Domain.Tests.Service;

public class OrdemCompraServiceTests
{
    private readonly ILogger<OrdemCompraService> _logger;
    private readonly IOrdemCompraRepository _ordemCompraRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ICustodiaMasterRepository _custodiaMasterRepository;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly OrdemCompraService _sut;

    public OrdemCompraServiceTests()
    {
        _logger = Substitute.For<ILogger<OrdemCompraService>>();
        _ordemCompraRepository = Substitute.For<IOrdemCompraRepository>();
        _cotacaoService = Substitute.For<ICotacaoService>();
        _custodiaMasterRepository = Substitute.For<ICustodiaMasterRepository>();
        _cestaRecomendadaRepository = Substitute.For<ICestaRecomendadaRepository>();
        _sut = new(_logger, _ordemCompraRepository, _cotacaoService, _custodiaMasterRepository, _cestaRecomendadaRepository);
    }

    [Fact]
    public async Task OrdemCompra_Deve_RetornarApplicationExcpetion_Quando_NaoTiverCestaRecomendada()
    {
        // Arrange
        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns((CestaRecomendada)null!);

        // Act
        var result = await _sut.EmitirOrdensDeCompraAsync(1000, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Value.Should().BeNull();
        result.Exception.Should().BeEquivalentTo(new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA"));
    }

    [Fact]
    public async Task OrdemCompra_Deve_RetornarException_Quando_FalharObterFechamento()
    {
        // Arrange
        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
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
    public async Task OrdemCompra_Deve_Gerar_Quando_Solicitado_E_SemErros()
    {
        // Arrange
        _cestaRecomendadaRepository.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(CestaRecomendada.CriarCesta("Name", new() { ComposicaoCesta.CriaItemNaCesta("PETR4", 30), ComposicaoCesta.CriaItemNaCesta("VALE3", 25), ComposicaoCesta.CriaItemNaCesta("ITUB4", 20), ComposicaoCesta.CriaItemNaCesta("BBDC4", 15), ComposicaoCesta.CriaItemNaCesta("WEGE3", 10) }));

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(Cotacao.CriarRegistro(DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 35), ComposicaoCotacao.CriarItem("VALE3", 62), ComposicaoCotacao.CriarItem("ITUB4", 30), ComposicaoCotacao.CriarItem("BBDC4", 15), ComposicaoCotacao.CriarItem("WEGE3", 40) }));

        _custodiaMasterRepository.ObterResiduosAsync(Arg.Any<CancellationToken>())
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
}