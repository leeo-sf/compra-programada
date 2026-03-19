using CompraProgramada.Domain.Entity;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class CotacaoTests
{

    [Fact]
    public async Task CriarRegistro_DeveRetornarCotacaoComSucesso_Quando_DadosValidosInformados()
    {
        var dataPregao = DateTime.Now;
        List<ComposicaoCotacao> composicao = new() { ComposicaoCotacao.CriarItem("PETR4", 42.83m) };
        var cotacao = Cotacao.CriarRegistro(dataPregao, composicao);

        cotacao.Id.Should().Be(0);
        cotacao.DataPregao.Should().Be(dataPregao);
        cotacao.ComposicaoCotacao.Should().NotBeEmpty();
    }
}