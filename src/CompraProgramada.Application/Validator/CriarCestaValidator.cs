using CompraProgramada.Shared.Request;
using FluentValidation;

namespace CompraProgramada.Application.Validator;

public sealed class CriarCestaValidator : AbstractValidator<CriarCestaRecomendadaRequest>
{
    public CriarCestaValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da cesta é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da cesta deve conter no máximo 100 caracteres.");

        RuleForEach(x => x.Itens)
            .SetValidator(new ComposicaoCestaValidator());
    }
}