using Domus.Application.Renters.Commands.InviteRenter;
using FluentValidation;

namespace Domus.Application.Renters.Commands.InviteRenter;

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