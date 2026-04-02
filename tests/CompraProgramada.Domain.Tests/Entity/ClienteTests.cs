using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Exceptions;
using FluentAssertions;
using System.Net;

namespace CompraProgramada.Domain.Tests.Entity;

public class ClienteTests
{
    private const string NOME = "Test Name";
    private const string CPF = "11111111111";
    private const string EMAIL = "test@email.com";

    [Fact]
    public async Task Criar_DeveRetornarClienteComSucesso_Quando_DadosValidosInformados()
    {
        decimal valorMensal = 100;
        var cliente = Cliente.Criar(new(NOME, CPF, EMAIL, valorMensal));

        cliente.Id.Should().Be(0);
        cliente.Nome.Should().Be(NOME);
        cliente.Cpf.Should().Be(CPF);
        cliente.Email.Should().Be(EMAIL);
        cliente.Ativo.Should().BeTrue();
        cliente.ValorMensal.Should().Be(valorMensal);
        cliente.ValorAnterior.Should().Be(valorMensal);
        cliente.ValorAporte.Should().Be(valorMensal / 3);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(80)]
    [InlineData(-10)]
    [InlineData(99.99)]
    public async Task Criar_DeveLancarValorMensalException_Quando_ValorMensal_MenorQueMinimoPermitido(decimal valorMensal)
    {
        var act = () => Cliente.Criar(new(NOME, CPF, EMAIL, valorMensal));
        var exception = act.Should().Throw<ValorMensalException>().Which;

        exception.Message.Should().Be("O valor mensal minimo e de R$ 100,00");
        exception.Codigo.Should().Be("VALOR_MENSAL_INVALIDO");
        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task AtualizarValorMensal_DeveAtualizaComSucesso_Quando_ValorMaiorQuePermitido(decimal novoValor)
    {
        var valorMensalAnterior = 200;
        var cliente = Cliente.Criar(new(NOME, CPF, EMAIL, valorMensalAnterior));

        cliente.AtualizarValorMensal(new(0, novoValor));

        cliente.ValorMensal.Should().Be(novoValor);
        cliente.ValorAnterior.Should().Be(valorMensalAnterior);
        cliente.ValorAporte.Should().Be(novoValor / 3);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(80)]
    [InlineData(-10)]
    [InlineData(99.99)]
    public async Task AtualizarValorMensal_DeveLancarValorMensalException_Quando_ValorMensal_MenorQueMinimoPermitido(decimal valorMensal)
    {
        var cliente = Cliente.Criar(new(NOME, CPF, EMAIL, 100));

        var act = () => cliente.AtualizarValorMensal(new(0, valorMensal));
        var exception = act.Should().Throw<ValorMensalException>().Which;

        exception.Message.Should().Be("O valor mensal minimo e de R$ 100,00");
        exception.Codigo.Should().Be("VALOR_MENSAL_INVALIDO");
        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Desativar_DeveDesativarComSucesso_Quando_ClienteAtivo()
    {
        var cliente = Cliente.Criar(new(NOME, CPF, EMAIL, 100));

        cliente.Desativar();

        cliente.Ativo.Should().BeFalse();
    }
}