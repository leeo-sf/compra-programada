using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Handler;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OperationResult;

public class AdministradorHandlerTests
{
    private readonly Mock<ICestaRecomendadaService> _cestaServiceMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly AdministradorHandler _handler;

    public AdministradorHandlerTests()
    {
        _cestaServiceMock = new Mock<ICestaRecomendadaService>();
        _clienteServiceMock = new Mock<IClienteService>();

        var logger = NullLogger<AdministradorHandler>.Instance;

        _handler = new AdministradorHandler(logger, _clienteServiceMock.Object, _cestaServiceMock.Object);
    }

    [Fact]
    public async Task Handle_CriarAlterarCesta_DeveRetornarSucesso_QuandoPrimeiraCesta()
    {
        var request = new CriarAlterarCestaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto("PETR4", 30),
                new ComposicaoCestaDto("VALE3", 25),
                new ComposicaoCestaDto("ITUB4", 20),
                new ComposicaoCestaDto("BBDC4", 15),
                new ComposicaoCestaDto("WEGE3", 10),
            }
        );

        var response = new CriarAlterarCestaDto
        {
            CestaAtualizada = false,
            CestaAtual = new CestaRecomandadaDto
            {
                Id = 1,
                Nome = "Nome",
                DataCriacao = DateTime.Now,
                DataDesativacao = DateTime.Now,
                Ativa = true,
                Itens = new List<ComposicaoCestaRecomendadaDto>
                {
                    new ComposicaoCestaRecomendadaDto { Id = 1, CestaId = 1, Ticker = "PETR4", Percentual = 30 },
                    new ComposicaoCestaRecomendadaDto { Id = 1, CestaId = 1, Ticker = "VALE3", Percentual = 25 },
                    new ComposicaoCestaRecomendadaDto { Id = 1, CestaId = 1, Ticker = "ITUB4", Percentual = 20 },
                    new ComposicaoCestaRecomendadaDto { Id = 1, CestaId = 1, Ticker = "BBDC4", Percentual = 15 },
                    new ComposicaoCestaRecomendadaDto { Id = 1, CestaId = 1, Ticker = "WEGE3", Percentual = 10 }
                }
            }
        };

        _cestaServiceMock.Setup(s => s.CriarCestaAsync(It.IsAny<CriarAlterarCestaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(response));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Mensagem.Should().Contain("Primeira cesta cadastrada");
    }

    [Fact]
    public async Task Handle_CriarCesta_Erro_NoServicoDeCesta()
    {
        var request = new CriarAlterarCestaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto("PETR4", 30),
                new ComposicaoCestaDto("VALE3", 25),
                new ComposicaoCestaDto("ITUB4", 20),
                new ComposicaoCestaDto("BBDC4", 15),
                new ComposicaoCestaDto("WEGE3", 10),
            }
        );

        var exception = new Exception("Falha crítica");
        _cestaServiceMock.Setup(s => s.CriarCestaAsync(It.IsAny<CriarAlterarCestaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Error<CriarAlterarCestaDto>(exception));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_CriarCesta_Sucesso_ComRebalanceamento()
    {
        var request = new CriarAlterarCestaRequest(
            "Cesta Top Five",
            new List<ComposicaoCestaDto>
            {
                new ComposicaoCestaDto("PETR4", 30),
                new ComposicaoCestaDto("VALE3", 25),
                new ComposicaoCestaDto("ITUB4", 20),
                new ComposicaoCestaDto("BBDC4", 15),
                new ComposicaoCestaDto("WEGE3", 10),
            }
        );

        var cestaAnterior = new CestaRecomandadaDto
        {
            Id = 1,
            Nome = "Carteira Top Match",
            DataCriacao = DateTime.UtcNow.AddDays(-10),
            Ativa = true,
            Itens = new List<ComposicaoCestaRecomendadaDto>
            {
                new() { Id = 1, CestaId = 1, Ticker = "ITUB4", Percentual = 50.0m },
                new() { Id = 1, CestaId = 1, Ticker = "WEGE3", Percentual = 50.0m }
            }
        };
        var cestaAtual = new CestaRecomandadaDto
        {
            Id = 2,
            Nome = "Carteira Plus",
            DataCriacao = DateTime.UtcNow,
            Ativa = true,
            Itens = new List<ComposicaoCestaRecomendadaDto>
            {
                new() { Id = 2, CestaId = 2, Ticker = "ITUB4", Percentual = 50.0m },
                new() { Id = 2, CestaId = 2, Ticker = "WEGE3", Percentual = 50.0m }
            }
        };

        var retornoServico = new CriarAlterarCestaDto
        {
            CestaAtualizada = true,
            CestaAtual = cestaAtual,
            CestaAnterior = cestaAnterior
        };

        _cestaServiceMock.Setup(s => s.CriarCestaAsync(It.IsAny<CriarAlterarCestaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(retornoServico));

        _clienteServiceMock.Setup(s => s.QuantidadeAtivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(100));

        var removidos = new List<string> { "OLD1" };
        var adicionados = new List<string> { "NEW1" };
        _cestaServiceMock.Setup(s => s.ObterMudancasDeAtivos(It.IsAny<List<ComposicaoCestaRecomendadaDto>>(), It.IsAny<List<ComposicaoCestaRecomendadaDto>>()))
            .Returns(Result.Success((removidos, adicionados)));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.RebalanceamentoDisparado.Should().BeTrue();
        result.Value.Mensagem.Should().Contain("Rebalanceamento disparado para 100 clientes ativos");
        result.Value.AtivosRemovidos.Should().BeEquivalentTo(removidos);
        result.Value.AtivosAdicionados.Should().BeEquivalentTo(adicionados);
    }

    [Fact]
    public async Task Handle_ObterCestaAtual_DeveRetornarCesta()
    {
        var cesta = new CestaRecomandadaDto
        {
            Id = 5,
            Nome = "Cesta Atual",
            DataCriacao = DateTime.UtcNow,
            Ativa = true,
            Itens = new List<ComposicaoCestaRecomendadaDto>
            {
                new() { Id = 1, CestaId = 5, Ticker = "PETR4", Percentual = 50 },
                new() { Id = 2, CestaId = 5, Ticker = "VALE3", Percentual = 50 }
            }
        };

        _cestaServiceMock.Setup(s => s.ObterCestaAtivaAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(cesta));

        var result = await _handler.Handle(new CestaAtualRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CestaId.Should().Be(cesta.Id);
        result.Value.Nome.Should().Be(cesta.Nome);
        result.Value.Itens.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_HistoricoCestas_DeveRetornarLista()
    {
        var lista = new List<CestaRecomandadaDto>
        {
            new() { Id = 1, Nome = "C1", DataCriacao = DateTime.UtcNow, Itens = new List<ComposicaoCestaRecomendadaDto>() },
            new() { Id = 2, Nome = "C2", DataCriacao = DateTime.UtcNow, Itens = new List<ComposicaoCestaRecomendadaDto>() }
        };

        _cestaServiceMock.Setup(s => s.HistoricoCestasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<CestaRecomandadaDto>>(lista));

        var result = await _handler.Handle(new CestaHistoricoRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}
