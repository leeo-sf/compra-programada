using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Handler.Api;
using CompraProgramada.Domain.Tests.TestUtils;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CompraProgramada.Domain.Tests.Handler;

public class CriarCestaHandlerTests
{
    private readonly ILogger<CriarCestaHandler> _logger;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly CriarCestaHandler _sut;

    public CriarCestaHandlerTests()
    {
        _logger = Substitute.For<ILogger<CriarCestaHandler>>();
        _cestaRecomendadaRepository = Substitute.For<ICestaRecomendadaRepository>();
        _clienteRepository = Substitute.For<IClienteRepository>();
        _sut = new(_logger, _cestaRecomendadaRepository, _clienteRepository);
    }

    [Fact]
    public async Task Handle_Deve_CriarCesta_Quando_NaoTiverUmaCestaAtiva()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        _cestaRecomendadaRepository
            .CriarAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(new CestaRecomendada(0, "", DateTime.MinValue, DateTime.MinValue, true, []));

        var result = await _sut.Handle(request, CancellationToken.None);

        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        response.Should().BeOfType<CriarCestaRecomendadaResponse>();
        response.CestaAnteriorDesativada.Should().BeNull();
        response.RebalanceamentoDisparado.Should().BeFalse();
        response.Mensagem.Should().Be("Primeira cesta cadastrada com sucesso.");
    }

    [Fact]
    public async Task Handle_Deve_CriarNovaCesta_E_DesativarAnterior_Quando_TiverUmaCestaAtiva()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAnterior = new CestaRecomendada(1, "Cesta Anterior", DateTime.MinValue, DateTime.MinValue, true, []);

        _cestaRecomendadaRepository
            .ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAnterior);

        _cestaRecomendadaRepository
            .CriarAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())
            .Returns(new CestaRecomendada(0, "", DateTime.MinValue, DateTime.MinValue, true, []));

        _clienteRepository.QuantidadeAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _sut.Handle(request, CancellationToken.None);

        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        response.Should().BeOfType<CriarCestaRecomendadaResponse>();
        response.CestaAnteriorDesativada.Should().NotBeNull();
        response.RebalanceamentoDisparado.Should().BeTrue();
        response.Mensagem.Should().Be("Cesta atualizada. Rebalanceamento disparado para 1 clientes ativos.");
    }

    [Theory]
    [MemberData(nameof(MudancaAtivosRequest))]
    public void MudancaAtivos_Deve_RetornarAtivosAdicionados_E_Removidos_Quando_HouverAlteracao(List<string> composicaoAnterior, List<string> ativosRemovidos, List<string> composicaoAtual, List<string> ativosAdicionados)
    {
        // Arrange & Act
        var (ativosRemovidosResult, ativosAdicionadosResult) = CriarCestaHandler.ObterMudancasDeAtivos(composicaoAnterior, composicaoAtual);

        // Assert
        ativosRemovidosResult.Should().NotBeEmpty();
        ativosAdicionadosResult.Should().NotBeEmpty();
        ativosRemovidosResult.Should().BeEqualTo(ativosRemovidos);
        ativosAdicionadosResult.Should().BeEqualTo(ativosAdicionados);
    }

    public static TheoryData<List<string>, List<string>, List<string>, List<string>> MudancaAtivosRequest()
    {
        return new()
        {
            {
                new List<string> { "PETR4", "AAPL4", "ITUB4" },
                new List<string> { "AAPL4", "ITUB4" },
                new List<string> { "PETR4", "TEST5", "WEGE3" },
                new List<string> { "TEST5", "WEGE3" }
            },
            {
                new List<string> { "PETR4", "VALE3", "RENT3" },
                new List<string> { "VALE3" },
                new List<string> { "PETR4", "ABEV3", "RENT3" },
                new List<string> { "ABEV3" }
            }
        };
    }
}