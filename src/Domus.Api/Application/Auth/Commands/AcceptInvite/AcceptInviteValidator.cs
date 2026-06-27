using FluentValidation;

namespace Domus.Application.Auth.Commands.AcceptInvite;

public sealed class AcceptInviteValidator : AbstractValidator<AcceptInviteCommand>
{
    public AcceptInviteValidator()
    {
        RuleFor(x => x.InviteToken).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}