using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Validator;
using FluentValidation.TestHelper;

namespace CompraProgramada.Application.Tests.Validator;

public class CriarCestaValidatorTests
{
    private readonly CriarCestaValidator _validator;

    public CriarCestaValidatorTests() => _validator = new CriarCestaValidator();

    [Fact]
    public void Deve_Passar_Quando_NomeInformado()
    {
        var command = new CriarCestaRecomendadaRequest("Name", new List<ComposicaoCestaDto> { new("PETR4", 30) });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Deve_Falhar_Quando_NomeVazio()
    {
        var command = new CriarCestaRecomendadaRequest("", new List<ComposicaoCestaDto> { new("PETR4", 30) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Nome)
            .WithErrorMessage("O nome da cesta é obrigatório.");
    }

    [Fact]
    public void Deve_Falhar_Quando_NomeMaior100_Caracteres()
    {
        var command = new CriarCestaRecomendadaRequest(new string('a', 101), new List<ComposicaoCestaDto> { new("PETR4", 30) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Nome)
            .WithErrorMessage("O nome da cesta deve conter no máximo 100 caracteres.");
    }

    [Fact]
    public void Deve_Falhar_Quando_NomeItemCesta_Vazio()
    {
        var command = new CriarCestaRecomendadaRequest("Nome", new List<ComposicaoCestaDto> { new("", 30) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Itens[0].Ticker")
            .WithErrorMessage("Nome do ativo deve ser preenchido.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Falhar_Quando_ParcentualItemCesta_InferiorAZero(decimal percentual)
    {
        var command = new CriarCestaRecomendadaRequest("Nome", new List<ComposicaoCestaDto> { new("", percentual) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Itens[0].Percentual")
            .WithErrorMessage("O percentual do ativo deve ser superior há zero.");
    }
}