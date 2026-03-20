using CompraProgramada.Application.Request;
using FluentValidation;

namespace CompraProgramada.Application.Validator;

public sealed class AdesaoValidator : AbstractValidator<AdesaoRequest>
{
    public AdesaoValidator()
    {
        RuleFor(ad => ad.Nome)
            .NotEmpty().WithMessage("O nome deve ser preenchido.");

        RuleFor(ad => ad.Cpf)
            .Matches("^[0-9]*$").WithMessage("O CPF deve conter apenas números.")
            .NotEmpty().Length(11).WithMessage("O CPF deve conter exatamente 11 caracteres.");

        RuleFor(ad => ad.Email)
            .NotEmpty().WithMessage("O email deve ser preenchido.")
            .EmailAddress().WithMessage("O e-mail deve ser válido!");

        RuleFor(ad => ad.ValorMensal)
            .GreaterThan(0).WithMessage("O valor mensal deve ser superior há zero.");
    }
}