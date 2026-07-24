using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Handler.Worker;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Domain.Tests.TestUtils;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CompraProgramada.Domain.Tests.Handler;

public class MotorCompraHandlerTests
{
    private readonly ILogger<MotorCompraHandler> _logger;
    private readonly IHistoricoExecucaoMotorRepository _historicoExecucaoMotorRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICalendarioMotorCompraService _calendarioMotorCompraService;
    private readonly IImpostoRendaService _impostoRendaService;
    private readonly IOrdemCompraService _ordemCompraService;
    private readonly ICustodiaMasterRepository _custodiaMasterRepository;
    private readonly OrdemCompraMapper _mapperOrdemCompra;
    private readonly DistribuicaoMapper _distribuicaoMapper;
    private readonly IDateTimeProvaider _dateTimeProvaider;
    private readonly MotorCompraHandler _sut;

    public MotorCompraHandlerTests()
    {
        _logger = Substitute.For<ILogger<MotorCompraHandler>>();
        _historicoExecucaoMotorRepository = Substitute.For<IHistoricoExecucaoMotorRepository>();
        _clienteRepository = Substitute.For<IClienteRepository>();
        _calendarioMotorCompraService = Substitute.For<ICalendarioMotorCompraService>();
        _impostoRendaService = Substitute.For<IImpostoRendaService>();
        _ordemCompraService = Substitute.For<IOrdemCompraService>();
        _custodiaMasterRepository = Substitute.For<ICustodiaMasterRepository>();
        _mapperOrdemCompra = Substitute.For<OrdemCompraMapper>();
        _distribuicaoMapper = Substitute.For<DistribuicaoMapper>();
        _dateTimeProvaider = Substitute.For<IDateTimeProvaider>();
        _sut = new MotorCompraHandler(_logger, _historicoExecucaoMotorRepository, _clienteRepository, _calendarioMotorCompraService, _impostoRendaService, _ordemCompraService, _custodiaMasterRepository, _mapperOrdemCompra, _distribuicaoMapper, _dateTimeProvaider);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoCompraForExecutadaComSucesso()
    {
        // Arrange
        var request = new ExecutarMotorCompraRequest(DateOnly.FromDateTime(DateTime.Now));
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();
        var ordensCompra = FakerRequest.OrdensCompraEmitidas();
        var distribuicoes = FakerRequest.Distribuicoes();
        var residuos = FakerRequest.ResiduosNaoDistribuidos();

        _clienteRepository.ObterClientesAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(clientesAtivos);

        _ordemCompraService.EmitirOrdensDeCompraAsync(Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(ordensCompra);

        _custodiaMasterRepository.ObterResiduosAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        _clienteRepository.AtualizarContasAsync(Arg.Any<List<ContaGrafica>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _custodiaMasterRepository.AtualizarResiduosAysnc(Arg.Any<List<CustodiaMaster>>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _impostoRendaService.PublicarIR(Arg.Any<List<Distribuicao>>(), Arg.Any<CancellationToken>())
            .Returns(10);

        _calendarioMotorCompraService.ObterDataReferenciaExecucao(Arg.Any<DateTime>())
            .Returns(DateTime.Now);

        _historicoExecucaoMotorRepository.SalvarHistoricoExecucaoAsync(Arg.Any<HistoricoExecucaoMotor>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<ExecutarMotorCompraResponse>();
    }

    [Fact]
    public async Task Handle_NaoDeve_ExecutarCompra_Quando_NaoEhDiaDeCompra()
    {
        var request = new ExecutarMotorCompraRequest(default);

        _calendarioMotorCompraService.DeveExecutarCompraHoje(Arg.Any<CancellationToken>())
            .Returns(false);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
        resultado.Value.Should().BeNull();
    }
}