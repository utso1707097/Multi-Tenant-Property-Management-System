using FluentValidation;

namespace Domus.Application.Auth.Commands.RegisterOwner;

public sealed class RegisterOwnerValidator : AbstractValidator<RegisterOwnerCommand>
{
    public RegisterOwnerValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
    }
}