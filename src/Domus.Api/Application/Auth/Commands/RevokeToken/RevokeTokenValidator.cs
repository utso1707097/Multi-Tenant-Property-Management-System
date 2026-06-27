using FluentValidation;

namespace Domus.Application.Auth.Commands.RevokeToken;

public sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}