using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Application.Validator;
using FluentValidation.TestHelper;

namespace CompraProgramada.Application.Tests.Validator;

public class CriarCestaValidatorTests
{
    private readonly CriarCestaValidator _validator;

    public CriarCestaValidatorTests() => _validator = new CriarCestaValidator();

    [Fact]
    public void Dado_RequestValida_Quando_Validate_DeveRetornarValido()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_Request_Com_NomeVazio_Quando_Validate_DeveRetornarError()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        var result = _validator.TestValidate(request with { Nome = "" });

        result.ShouldHaveValidationErrorFor(x => x.Nome)
            .WithErrorMessage("O nome da cesta é obrigatório.").Only();
    }

    [Fact]
    public void Dado_Request_Com_NomeMaior100Caracteres_Quando_Validate_DeveRetornarError()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        var result = _validator.TestValidate(request with { Nome = new string('a', 101) });

        result.ShouldHaveValidationErrorFor(x => x.Nome)
            .WithErrorMessage("O nome da cesta deve conter no máximo 100 caracteres.").Only();
    }

    [Fact]
    public void Dado_Request_Com_NomeItemVazio_Quando_Validate_DeveRetornarError()
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        var result = _validator.TestValidate(request with { Itens = new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "", Percentual = 30 } } });

        result.ShouldHaveValidationErrorFor("Itens[0].Ticker")
            .WithErrorMessage("Nome do ativo deve ser preenchido.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Dado_Request_Com_PercentualItemInferiorAZero_Quando_Validate_DeveRetornarError(decimal percentual)
    {
        var request = FakerRequest.CriarCestaRecomendadaRequest();

        var result = _validator.TestValidate(request with { Itens = new List<ComposicaoCestaDto> { new ComposicaoCestaDto { Ticker = "TEST4", Percentual = percentual } } });

        result.ShouldHaveValidationErrorFor("Itens[0].Percentual")
            .WithErrorMessage("O percentual do ativo deve ser superior há zero.");
    }
}