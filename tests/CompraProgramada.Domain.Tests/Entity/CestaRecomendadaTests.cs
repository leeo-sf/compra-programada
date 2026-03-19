using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.Tests.TestsUtils;
using FluentAssertions;
using System.Net;

namespace CompraProgramada.Domain.Tests.Entity;

public class CestaRecomendadaTests
{
    private const string NOME = "Cesta x";

    [Theory]
    [MemberData(nameof(CestaRecomendadaFaker.ComposicaoCestaDadosValidos), MemberType = typeof(CestaRecomendadaFaker))]
    public void CriarCesta_DeveRetornarCestaRecomendadaComSucesso_Quando_DadosVaildosInformados(List<ComposicaoCesta> composicaoCesta)
    {
        var cestaRecomendada = CestaRecomendada.CriarCesta(NOME, composicaoCesta);

        cestaRecomendada.Id.Should().Be(0);
        cestaRecomendada.Nome.Should().Be(NOME.ToUpper());
        cestaRecomendada.Ativa.Should().BeTrue();
        cestaRecomendada.ComposicaoCesta.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(CestaRecomendadaFaker.ComposicaoCestaQtdInvalida), MemberType = typeof(CestaRecomendadaFaker))]
    public void CriarCesta_DeveLancarQuantidadeItensCestaException_Quando_QuantidadeItensCesta_InferiorAoPermitido(List<ComposicaoCesta> composicaoCesta)
    {
        var qtdItens = composicaoCesta.Count;
        var act = () => CestaRecomendada.CriarCesta(NOME, composicaoCesta);
        var exception = act.Should().Throw<QuantidadeItensCestaException>().Which;

        exception.Message.Should().Be($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {qtdItens}.");
        exception.Codigo.Should().Be("QUANTIDADE_ATIVOS_INVALIDA");
        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(CestaRecomendadaFaker.ComposicaoCestaPercentualInvalida), MemberType = typeof(CestaRecomendadaFaker))]
    public void CriarCesta_DeveLancarPercentualCestaException_Quando_SomaPercentualItensCesta_InferiorAoPermitido(List<ComposicaoCesta> composicaoCesta)
    {
        var somaPercentual = composicaoCesta.Sum(x => x.Percentual);
        var act = () => CestaRecomendada.CriarCesta(NOME, composicaoCesta);
        var exception = act.Should().Throw<PercentualCestaException>().Which;

        exception.Message.Should().Be($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {somaPercentual}%.");
        exception.Codigo.Should().Be("PERCENTUAIS_INVALIDOS");
        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(CestaRecomendadaFaker.CestaRecomendadaDadosValidos), MemberType = typeof(CestaRecomendadaFaker))]
    public void DesativarCesta_DeveRetornarCestaDesativadaComSucesso_Quando_CestaAtiva(CestaRecomendada cestaAtual)
    {
        cestaAtual.DesativarCesta();

        cestaAtual.Id.Should().Be(0);
        cestaAtual.Nome.Should().NotBeEmpty();
        cestaAtual.Ativa.Should().BeFalse();
        cestaAtual.ComposicaoCesta.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(CestaRecomendadaFaker.CestaRecomendadaInativa), MemberType = typeof(CestaRecomendadaFaker))]
    public void DesativarCesta_DeveLancarApplicationException_Quando_CestaInativa(CestaRecomendada cestaAtual)
    {
        var act = () => cestaAtual.DesativarCesta();

        act.Should().Throw<ApplicationException>().WithMessage("A cesta já se encontra desativada.");
    }
}