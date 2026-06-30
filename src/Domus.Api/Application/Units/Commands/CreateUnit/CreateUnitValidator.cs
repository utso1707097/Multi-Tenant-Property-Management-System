using FluentValidation;

namespace Domus.Application.Units.Commands.CreateUnit;

public sealed class CreateUnitValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitValidator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();

        RuleFor(x => x.UnitNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.RentAmount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .MaximumLength(3);
    }
}