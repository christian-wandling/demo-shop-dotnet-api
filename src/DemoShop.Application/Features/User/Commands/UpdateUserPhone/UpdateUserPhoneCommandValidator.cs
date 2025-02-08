#region

using DemoShop.Domain.Common.Constants;
using FluentValidation;

#endregion

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed class UpdateUserPhoneCommandValidator : AbstractValidator<UpdateUserPhoneCommand>
{
    public UpdateUserPhoneCommandValidator()
    {
        RuleFor(x => x.UpdateUser)
            .NotNull();

        RuleFor(x => x.UpdateUser.Phone)
            .NotEmpty()
            .Matches(ValidationConstants.PhoneNumber)
            .WithMessage("Invalid phone number");
    }
}
