#region

using FluentValidation;

#endregion

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserIdentity.KeycloakUserId)
            .NotEmpty();

        RuleFor(x => x.UserIdentity.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.UserIdentity.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UserIdentity.LastName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
