using CompraProgramada.Application.Request;
using FluentValidation;

namespace CompraProgramada.Application.Validator;

public sealed class AtualizarValorMensalValidator : AbstractValidator<AtualizarValorMensalRequest>
{
    public AtualizarValorMensalValidator()
        => RuleFor(ad => ad.NovoValorMensal)
            .GreaterThan(0).WithMessage("O novo valor mensal deve ser superior há zero.");
}