using CompraProgramada.Application.Dto;
using FluentValidation;

namespace CompraProgramada.Application.Validator;

internal sealed class ComposicaoCestaValidator : AbstractValidator<ComposicaoCestaDto>
{
    public ComposicaoCestaValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty().WithMessage("Nome da ação deve ser preenchido.");

        RuleFor(x => x.Percentual)
            .GreaterThan(0).WithMessage("O percentual do ativo deve ser superior há zero.");
    }
}