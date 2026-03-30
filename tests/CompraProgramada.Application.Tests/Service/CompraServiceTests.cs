using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Response;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class CompraServiceTests
{
    private readonly ILogger<CompraService> _logger;
    private readonly IHistoricoExecucaoMotorService _historicoExecucaoService;
    private readonly IClienteService _clienteService;
    private readonly ICalendarioMotorCompraService _calendarioMotorCompraService;
    private readonly IDistribuicaoService _distribuicaoService;
    private readonly IImpostoRendaService _impostoRendaService;
    private readonly IOrdemCompraService _ordemCompraService;
    private readonly ICustodiaMasterService _custodiaMasterService;
    private readonly OrdemCompraMapper _mapperOrdemCompra;
    private readonly DistribuicaoMapper _mapperDistribuicao;
    private readonly CompraService _sut;

    public CompraServiceTests()
    {
        _logger = Substitute.For<ILogger<CompraService>>();
        _historicoExecucaoService = Substitute.For<IHistoricoExecucaoMotorService>();
        _clienteService = Substitute.For<IClienteService>();
        _calendarioMotorCompraService = Substitute.For<ICalendarioMotorCompraService>();
        _distribuicaoService = Substitute.For<IDistribuicaoService>();
        _impostoRendaService = Substitute.For<IImpostoRendaService>();
        _ordemCompraService = Substitute.For<IOrdemCompraService>();
        _custodiaMasterService = Substitute.For<ICustodiaMasterService>();
        _mapperOrdemCompra = Substitute.For<OrdemCompraMapper>();
        _mapperDistribuicao = Substitute.For<DistribuicaoMapper>(Substitute.For<ContaMapper>(), Substitute.For<HistoricoCompraMapper>(), Substitute.For<CustodiaFilhoteMapper>());
        _sut = new(_logger, _historicoExecucaoService, _clienteService, _calendarioMotorCompraService, _distribuicaoService, _impostoRendaService, _ordemCompraService, _custodiaMasterService, _mapperOrdemCompra, _mapperDistribuicao);
    }

    [Fact]
    public async Task ExecutarCompra_Deve_EfetuarCompraComSucesso_Quando_NaoOcorrerErros()
    {
        // Arrange
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();
        var ordensCompra = FakerRequest.OrdensCompraEmitidas();
        var distribuicoes = FakerRequest.Distribuicoes();
        var residuos = FakerRequest.ResiduosNaoDistribuidos();

        _clienteService.ObtemClientesAtivoAsync(Arg.Any<CancellationToken>())
            .Returns(clientesAtivos);

        _ordemCompraService.EmitirOrdensDeCompraAsync(Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(ordensCompra);

        _distribuicaoService.DistribuirParaCustodiasAsync(Arg.Any<List<Cliente>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(distribuicoes);

        _custodiaMasterService.CapturarResiduosNaoDistribuidosAsync(Arg.Any<List<Distribuicao>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<CancellationToken>())
            .Returns(residuos);

        _impostoRendaService.PublicarIR(Arg.Any<List<Distribuicao>>(), Arg.Any<CancellationToken>())
            .Returns(10);

        _calendarioMotorCompraService.ObterDataReferenciaExecucao(Arg.Any<DateTime>())
            .Returns(DateTime.Now);

        // Act
        var result = await _sut.ExecutarCompraAsync(DateTime.Now, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<ExecutarCompraResponse>();
    }

    [Fact]
    public async Task ExecutarCompra_Deve_NaoExecutarCompra_Quando_EhDiaDeExecucao()
    {
        // Arrange
        _historicoExecucaoService.ExecutarCompraHojeAsync(Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _sut.ExecutarCompraAsync(null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecutarCompra_Deve_NaoExecutarCompra_Quando_NaoExisteClientesAtivos()
    {
        // Arrange
        _historicoExecucaoService.ExecutarCompraHojeAsync(Arg.Any<CancellationToken>())
            .Returns(true);

        _clienteService.ObtemClientesAtivoAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Cliente>());

        // Act
        var result = await _sut.ExecutarCompraAsync(null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<CompraException>();
        result.Exception.Message.Should().Be("Nenhum cliente ativo cadastrado");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecutarCompra_Deve_Falhar_Quando_EmitirOrdensCompra_Falhar()
    {
        // Arrange
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();

        _clienteService.ObtemClientesAtivoAsync(Arg.Any<CancellationToken>())
            .Returns(clientesAtivos);

        _ordemCompraService.EmitirOrdensDeCompraAsync(Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(new Exception("Error"));

        // Act
        var result = await _sut.ExecutarCompraAsync(DateTime.Now, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception.Message.Should().Be("Error");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecutarCompra_Deve_Falhar_Quando_DistribuirParaCustodias_Falhar()
    {
        // Arrange
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();
        var ordensCompra = FakerRequest.OrdensCompraEmitidas();

        _clienteService.ObtemClientesAtivoAsync(Arg.Any<CancellationToken>())
            .Returns(clientesAtivos);

        _ordemCompraService.EmitirOrdensDeCompraAsync(Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(ordensCompra);

        _distribuicaoService.DistribuirParaCustodiasAsync(Arg.Any<List<Cliente>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new Exception("Error"));

        // Act
        var result = await _sut.ExecutarCompraAsync(DateTime.Now, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception.Message.Should().Be("Error");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecutarCompra_Deve_Falhar_Quando_CapturarResiduos_Falhar()
    {
        // Arrange
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();
        var ordensCompra = FakerRequest.OrdensCompraEmitidas();
        var distribuicoes = FakerRequest.Distribuicoes();

        _clienteService.ObtemClientesAtivoAsync(Arg.Any<CancellationToken>())
            .Returns(clientesAtivos);

        _ordemCompraService.EmitirOrdensDeCompraAsync(Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(ordensCompra);

        _distribuicaoService.DistribuirParaCustodiasAsync(Arg.Any<List<Cliente>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(distribuicoes);

        _custodiaMasterService.CapturarResiduosNaoDistribuidosAsync(Arg.Any<List<Distribuicao>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<CancellationToken>())
            .Returns(new Exception("Error"));

        // Act
        var result = await _sut.ExecutarCompraAsync(DateTime.Now, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception.Message.Should().Be("Error");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecutarCompra_Deve_Falhar_Quando_PublicarIR_Falhar()
    {
        // Arrange
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();
        var ordensCompra = FakerRequest.OrdensCompraEmitidas();
        var distribuicoes = FakerRequest.Distribuicoes();
        var residuos = FakerRequest.ResiduosNaoDistribuidos();

        _clienteService.ObtemClientesAtivoAsync(Arg.Any<CancellationToken>())
            .Returns(clientesAtivos);

        _ordemCompraService.EmitirOrdensDeCompraAsync(Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(ordensCompra);

        _distribuicaoService.DistribuirParaCustodiasAsync(Arg.Any<List<Cliente>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(distribuicoes);

        _custodiaMasterService.CapturarResiduosNaoDistribuidosAsync(Arg.Any<List<Distribuicao>>(), Arg.Any<List<OrdemCompra>>(), Arg.Any<CancellationToken>())
            .Returns(residuos);

        _impostoRendaService.PublicarIR(Arg.Any<List<Distribuicao>>(), Arg.Any<CancellationToken>())
            .Returns(new Exception("Error"));

        // Act
        var result = await _sut.ExecutarCompraAsync(DateTime.Now, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception.Message.Should().Be("Error");
        result.Value.Should().BeNull();
    }
}