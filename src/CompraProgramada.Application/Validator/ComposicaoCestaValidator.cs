using CompraProgramada.Shared.Dto;
using FluentValidation;

namespace CompraProgramada.Application.Validator;

public sealed class ComposicaoCestaValidator : AbstractValidator<ComposicaoCestaDto>
{
    public ComposicaoCestaValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty().WithMessage("Nome do ativo deve ser preenchido.");

        RuleFor(x => x.Percentual)
            .GreaterThan(0).WithMessage("O percentual do ativo deve ser superior há zero.");
    }
}