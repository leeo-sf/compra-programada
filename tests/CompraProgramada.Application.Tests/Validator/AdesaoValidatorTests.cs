using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Application.Validator;
using FluentValidation.TestHelper;

namespace CompraProgramada.Application.Tests.Validator;

public class AdesaoValidatorTests
{
    private readonly AdesaoValidator _sut = new();

    [Fact]
    public void Dado_RequestValida_Quando_Validate_DeveRetornarValido()
    {
        var command = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_Request_Com_NomeVazio_Quando_Validate_DeveRetornarError()
    {
        var request = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(request with { Nome = "" });

        result.ShouldHaveValidationErrorFor(x => x.Nome)
            .WithErrorMessage("O nome deve ser preenchido.")
            .WithErrorCode("NotEmptyValidator").Only();
    }

    [Fact]
    public void Dado_Request_Com_CaracteresCpf_Quando_Validate_DeveRetornarError()
    {
        var request = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(request with { Cpf = "123.184.110" });

        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("O CPF deve conter apenas números.")
            .WithErrorCode("RegularExpressionValidator").Only();
    }

    [Theory]
    [InlineData("123456789011")]
    [InlineData("123.184.110-80")]
    [InlineData("123")]
    public void Dado_Request_Com_CpfDiferenteDe11Digitos_Quando_Validate_DeveRetornarError(string cpf)
    {
        var request = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(request with { Cpf = cpf });

        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("O CPF deve conter exatamente 11 caracteres.")
            .WithErrorCode("ExactLengthValidator");
    }

    [Fact]
    public void Dado_Request_Com_EmailVazio_Quando_Validate_DeveRetornarError()
    {
        var request = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(request with { Email = "" });

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ser preenchido.")
            .WithErrorCode("NotEmptyValidator");
    }

    [Theory]
    [InlineData("email.com.br")]
    [InlineData("emailtest")]
    [InlineData("email$test")]
    public void Dado_Request_Com_EmailInvalido_Quando_Validate_DeveRetornarError(string email)
    {
        var request = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(request with { Email = email });

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O e-mail deve ser válido!")
            .WithErrorCode("EmailValidator").Only();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Dado_Request_Com_ValorMensalInvalido_Quando_Validate_DeveRetornarError(decimal valorMensal)
    {
        var request = FakerRequest.AdesaoRequest().Generate();

        var result = _sut.TestValidate(request with { ValorMensal = valorMensal });

        result.ShouldHaveValidationErrorFor(x => x.ValorMensal)
            .WithErrorMessage("O valor mensal deve ser superior há zero.")
            .WithErrorCode("GreaterThanValidator").Only();
    }
}