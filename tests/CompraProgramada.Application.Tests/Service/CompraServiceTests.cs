using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class CompraServiceTests
{
    private readonly Mock<IHistoricoExecucaoMotorService> _historicoMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly Mock<ICalendarioMotorCompraService> _calendarioMock;
    private readonly Mock<IDistribuicaoService> _distribuicaoMock;
    private readonly Mock<IImpostoRendaService> _impostoMock;
    private readonly Mock<IOrdemCompraService> _ordemMock;
    private readonly Mock<ICustodiaMasterService> _custodiaMasterMock;
    private readonly CompraService _service;

    public CompraServiceTests()
    {
        _historicoMock = new Mock<IHistoricoExecucaoMotorService>();
        _clienteServiceMock = new Mock<IClienteService>();
        _calendarioMock = new Mock<ICalendarioMotorCompraService>();
        _distribuicaoMock = new Mock<IDistribuicaoService>();
        _impostoMock = new Mock<IImpostoRendaService>();
        _ordemMock = new Mock<IOrdemCompraService>();
        _custodiaMasterMock = new Mock<ICustodiaMasterService>();

        var logger = NullLogger<CompraService>.Instance;

        _service = new CompraService(
            logger,
            _historicoMock.Object,
            _clienteServiceMock.Object,
            _calendarioMock.Object,
            _distribuicaoMock.Object,
            _impostoMock.Object,
            _ordemMock.Object,
            _custodiaMasterMock.Object);
    }

    [Fact]
    public async Task SeparacaoLoteDeCompraAsync_DeveRetornarErro_QuandoListaVazia()
    {
        var result = await _service.SeparacaoLoteDeCompraAsync(new List<FechamentoAtivoB3Dto>(), DateTime.Now, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SeparacaoLoteDeCompraAsync_DeveLancarExcecao_QuandoNaoObterResiduos()
    {
        _custodiaMasterMock.Setup(c => c.ObterResiduosNaoDistribuidos(It.IsAny<CancellationToken>()))!
            .ReturnsAsync(Result.Error<List<CustodiaMasterDto>>(new Exception("erro")));

        var fechamento = new FechamentoAtivoB3Dto("TICK", 10m, 100m);

        await Assert.ThrowsAsync<Exception>(async () => await _service.SeparacaoLoteDeCompraAsync(new List<FechamentoAtivoB3Dto> { fechamento }, DateTime.Now, CancellationToken.None));
    }

    [Fact]
    public async Task SeparacaoLoteDeCompraAsync_DeveGerarOrdens_QuandoExistiremResiduos()
    {
        var fechamento = new FechamentoAtivoB3Dto("TICK", 10m, 1000m);

        var custodia = new CustodiaMasterDto { Id = 1, ContaMasterId = 1, Ticker = "TICK", QuantidadeResiduos = 500 };
        _custodiaMasterMock.Setup(c => c.ObterResiduosNaoDistribuidos(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Success(new List<CustodiaMasterDto> { custodia }));
        _custodiaMasterMock.Setup(c => c.SubtrairResiduosParaCompra(It.IsAny<CustodiaMasterDto>(), It.IsAny<int>())).Returns(300);

        var result = await _service.SeparacaoLoteDeCompraAsync(new List<FechamentoAtivoB3Dto> { fechamento }, DateTime.Now, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Ticker.Should().Be("TICK");
    }

    [Fact]
    public async Task ExecutarCompraAsync_DeveLancarQuandoObterClientesFalha()
    {
        var ex = new Exception("erro");
        _clienteServiceMock.Setup(c => c.ObtemClientesAtivoAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Error<List<ClienteDto>>(ex));

        await Assert.ThrowsAsync<Exception>(async () => await _service.ExecutarCompraAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task ExecutarCompraAsync_FlowCompleto_DeveRetornarResultado()
    {
        var clientes = new List<ClienteDto>
        {
            new ClienteDto { ClienteId = 1, Nome = "C1", Cpf = "111", Email = "e@e", ValorAnterior = 0m, ValorMensal = 150m, Ativo = true, DataAdesao = DateTime.UtcNow, ContaGrafica = new ContaGraficaDto(1, "000", DateTime.UtcNow, 1, "Tipo", new List<HistoricoCompraDto>(), new List<CustodiaFilhoteDto>()) }
        };

        _clienteServiceMock.Setup(c => c.ObtemClientesAtivoAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(clientes));
        _clienteServiceMock.Setup(c => c.TotalConsolidade(It.IsAny<List<ClienteDto>>())).Returns(Result.Success(100m));

        var grupoAtivos = new List<AtivoAhCompraDto> { new AtivoAhCompraDto("TICK", 100, 10m) };
        var fechamentos = new List<FechamentoAtivoB3Dto> { new FechamentoAtivoB3Dto("TICK", 10m, 1000m) };

        _distribuicaoMock.Setup(d => d.DistribuirGrupoAtivoAsync(It.IsAny<List<ClienteDto>>(), It.IsAny<decimal>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((grupoAtivos, fechamentos));

        var custodia = new CustodiaMasterDto { Id = 1, ContaMasterId = 1, Ticker = "TICK", QuantidadeResiduos = 1000 };
        _custodiaMasterMock.Setup(c => c.ObterResiduosNaoDistribuidos(It.IsAny<CancellationToken>()))!.ReturnsAsync(Result.Success(new List<CustodiaMasterDto> { custodia }));
        _custodiaMasterMock.Setup(c => c.SubtrairResiduosParaCompra(It.IsAny<CustodiaMasterDto>(), It.IsAny<int>())).Returns(100);

        var ordens = new List<OrdemCompraDto> { new OrdemCompraDto(1, "TICK", 100, new List<DetalheOrdemCompraDto> { new DetalheOrdemCompraDto("PADRAO", "TICK", 100) }, 10m) };
        _ordemMock.Setup(o => o.RegistrarOrdensDeCompraAsync(It.IsAny<List<OrdemCompraDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(ordens));

        var distribuicoes = new List<DistribuicaoDto> { new DistribuicaoDto(0, "111", 1, 1, "TICK", 100, 1000m, new ContaGraficaDto(1, "000", DateTime.UtcNow, 1, "Tipo", new List<HistoricoCompraDto>(), new List<CustodiaFilhoteDto>()), DateTime.UtcNow, 1, "C1", 50m, new List<AtivoDto> { new AtivoDto("TICK", 100) }) };
        _distribuicaoMock.Setup(d => d.DistribuirCustodiasAsync(It.IsAny<List<ClienteDto>>(), It.IsAny<List<AtivoAhCompraDto>>(), It.IsAny<decimal>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(distribuicoes));
        _distribuicaoMock.Setup(d => d.SalvarDistribuicoesAsync(It.IsAny<List<DistribuicaoDto>>(), It.IsAny<List<OrdemCompraDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());

        _custodiaMasterMock.Setup(c => c.CapturarResiduosDeCustodiaDistribuida(It.IsAny<List<AtivoAhCompraDto>>(), It.IsAny<List<DistribuicaoDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(new List<ResiduoCustodiaMasterDto>()));

        _impostoMock.Setup(i => i.CalcularIRDedoDuro(It.IsAny<List<DistribuicaoDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(0));

        _calendarioMock.Setup(c => c.ObterDataReferenciaExecucao(It.IsAny<DateTime>())).Returns(DateTime.UtcNow);
        _historicoMock.Setup(h => h.SalvarExecucaoAsync(It.IsAny<ExecucaoMotorCompraDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.ExecutarCompraAsync(DateTime.UtcNow, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalClientes.Should().Be(1);
    }
}