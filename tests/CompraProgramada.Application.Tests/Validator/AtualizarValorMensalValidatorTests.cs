using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Application.Validator;
using FluentValidation.TestHelper;

namespace CompraProgramada.Application.Tests.Validator;

public class AtualizarValorMensalValidatorTests
{
    private readonly AtualizarValorMensalValidator _validator = new();

    [Fact]
    public void Dado_RequestValida_Quando_Validate_DeveRetornarValido()
    {
        var request = FakerRequest.AtualizarValorMensalRequest().Generate();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Deve_Falhar_Quando_ParcentualItemCesta_InferiorAZero(decimal novoValorMensal)
    {
        var request = FakerRequest.AtualizarValorMensalRequest().Generate();

        var result = _validator.TestValidate(request with { NovoValorMensal = novoValorMensal });

        result.ShouldHaveValidationErrorFor(x => x.NovoValorMensal)
            .WithErrorMessage("O novo valor mensal deve ser superior há zero.").Only();
    }
}