using FluentValidation;

namespace CompraProgramada.Shared.Request.Validator;

public sealed class AtualizarValorMensalValidator : AbstractValidator<AtualizarValorMensalRequest>
{
    public AtualizarValorMensalValidator()
        => RuleFor(ad => ad.NovoValorMensal)
            .GreaterThan(0).WithMessage("O novo valor mensal deve ser superior há zero.");
}