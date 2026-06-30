using Domus.Application.Renters.Commands.InviteRenter;
using FluentValidation;

public sealed class InviteRenterValidator : AbstractValidator<InviteRenterCommand>
{
    public InviteRenterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(120);
    }
}