using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using NSubstitute;

namespace CompraProgramada.Application.Tests.Service;

public class CustodiaMasterServiceTests
{
    private readonly ICustodiaMasterRepository _custodiaMasterRepository;
    private readonly CustodiaMasterService _sut;

    public CustodiaMasterServiceTests()
    {
        _custodiaMasterRepository = Substitute.For<ICustodiaMasterRepository>();
        _sut = new(_custodiaMasterRepository);
    }

    [Fact]
    public async Task CustodiaMaster_Deve_RetornarResiduos_Quando_TiveremResiduos()
    {
        // Arrange
        var custodias = new List<CustodiaMaster>() { CustodiaMaster.CriarCustodia(1, "PETR4"), CustodiaMaster.CriarCustodia(1, "APPL4") };

        _custodiaMasterRepository.ObterResiduosAsync(Arg.Any<CancellationToken>())
            .Returns(custodias);

        // Act
        var result = await _sut.ObterResiduosNaoDistribuidos(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task CustodiaMaster_Deve_RetornarNull_Quando_NaoTiveremResiduos()
    {
        // Arrange
        _custodiaMasterRepository.ObterResiduosAsync(Arg.Any<CancellationToken>())
            .Returns((List<CustodiaMaster>)null!);

        // Act
        var result = await _sut.ObterResiduosNaoDistribuidos(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task CustodiaMaster_Deve_RetornarSucesso_Quando_AtualizarCustodias()
    {
        // Arrange
        var custodias = new List<CustodiaMaster>() { CustodiaMaster.CriarCustodia(1, "PETR4"), CustodiaMaster.CriarCustodia(1, "APPL4") };

        _custodiaMasterRepository.AtualizarResiduosAysnc(Arg.Any<List<CustodiaMaster>>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AtualizarResiduosAsync(custodias, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
    }

    [Fact]
    public async Task CustodiaMaster_Deve_RetornarApplicationException_Quando_AtualizarCustodias_Vazio()
    {
        // Arrange
        var custodias = new List<CustodiaMaster>() {  };

        // Act
        var result = await _sut.AtualizarResiduosAsync(custodias, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Um grupo para compra deve ser informado.");
    }

    [Theory]
    [MemberData(nameof(CapturarResiduos))]
    public async Task CustodiaMaster_Deve_CapturarResiduos_Quando_AtivosNaoDistribuidos(List<CustodiaMaster> residuosAtuais, List<Distribuicao> distribuicoes, List<OrdemCompra> ordensCompra, List<AtivoQuantidadeDto> residuos)
    {
        // Arrange
        _custodiaMasterRepository.ObterResiduosAsync(Arg.Any<CancellationToken>())
            .Returns(residuosAtuais);

        // Act
        var result = await _sut.CapturarResiduosNaoDistribuidosAsync(distribuicoes, ordensCompra, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().BeEquivalentTo(residuos);
    }

    public static TheoryData<List<CustodiaMaster>, List<Distribuicao>, List<OrdemCompra>, List<AtivoQuantidadeDto>> CapturarResiduos()
        => new()
        {
            {
                new() { },
                FakerRequest.Distribuicoes(),
                FakerRequest.OrdensCompraEmitidas(),
                new() { new AtivoQuantidadeDto { Ticker = "PETR4", Quantidade = 1 }, new AtivoQuantidadeDto { Ticker = "VALE3", Quantidade = 0 }, new AtivoQuantidadeDto { Ticker = "ITUB4", Quantidade = 1 }, new AtivoQuantidadeDto { Ticker = "BBDC4", Quantidade = 0 }, new AtivoQuantidadeDto { Ticker = "WEGE3", Quantidade = 1 } }
            },
            {
                new() { new CustodiaMaster(0, 0, "PETR4", 6, new(0, new())), new CustodiaMaster(0, 0, "VALE3", 4, new(0, new())), new CustodiaMaster(0, 0, "ITUB4", 1, new(0, new())), new CustodiaMaster(0, 0, "BBDC4", 8, new(0, new())) },
                new()
                {
                    new(0, 0, 0, "PETR4", 13, 100, new(0, "PETR4", 24, 35, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "PETR4", 9, 100, new(0, "PETR4", 24, 35, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "PETR4", 5, 100, new(0, "PETR4", 24, 35, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "VALE3", 2, 100, new(0, "PETR4", 14, 62, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "VALE3", 7, 100, new(0, "PETR4", 14, 62, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "VALE3", 4, 100, new(0, "PETR4", 14, 62, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "ITUB4", 3, 100, new(0, "ITUB4", 23, 30, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "ITUB4", 10, 100, new(0, "ITUB4", 23, 30, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "ITUB4", 3, 100, new(0, "ITUB4", 23, 30, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "BBDC4", 12, 100, new(0, "BBDC4", 35, 15, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "BBDC4", 11, 100, new(0, "BBDC4", 35, 15, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                    new(0, 0, 0, "BBDC4", 9, 100, new(0, "BBDC4", 35, 15, 100, DateTime.Now), new(new(0, "name", "11111111111", "email@teste.com", 100, 100, true, DateTime.Now))),
                },
                new()
                {
                    OrdemCompra.GerarOrdemCompra("PETR4", 24, 35),
                    OrdemCompra.GerarOrdemCompra("VALE3", 14, 62),
                    OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30),
                    OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)
                },
                new() { new AtivoQuantidadeDto { Ticker = "PETR4", Quantidade = 3 }, new AtivoQuantidadeDto { Ticker = "VALE3", Quantidade = 5 }, new AtivoQuantidadeDto { Ticker = "ITUB4", Quantidade = 8 }, new AtivoQuantidadeDto { Ticker = "BBDC4", Quantidade = 11 } }
            }
        };
}