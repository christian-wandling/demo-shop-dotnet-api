using DemoShop.Domain.Common.Constants;
using FluentValidation;

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed class UpdateUserPhoneValidator : AbstractValidator<UpdateUserPhoneCommand>
{
    public UpdateUserPhoneValidator()
    {
        RuleFor(x => x.UserIdentity.KeycloakUserId)
            .NotEmpty();

        RuleFor(x => x.NewPhoneNumber)
            .NotEmpty()
            .Matches(ValidationConstants.PhoneNumber())
            .WithMessage("Invalid phone number");
    }
}
