using FluentValidation;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.KeycloakUserId)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Firstname)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Lastname)
            .NotEmpty()
            .MaximumLength(100);
    }
}
