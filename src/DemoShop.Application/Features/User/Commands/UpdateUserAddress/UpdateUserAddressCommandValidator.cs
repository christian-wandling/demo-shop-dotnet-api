#region

using FluentValidation;

#endregion

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressCommandValidator : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserAddressCommandValidator()
    {
        RuleFor(x => x.UpdateUserAddress)
            .NotEmpty();

        RuleFor(x => x.UpdateUserAddress.Street)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.UpdateUserAddress.Apartment)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.UpdateUserAddress.City)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.UpdateUserAddress.Zip)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.UpdateUserAddress.Country)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.UpdateUserAddress.Region)
            .MaximumLength(50);
    }
}
