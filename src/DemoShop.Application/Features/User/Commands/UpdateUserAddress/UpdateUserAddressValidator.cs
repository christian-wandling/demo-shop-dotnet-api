using FluentValidation;

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed class UpdateUserPhoneValidator : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserPhoneValidator()
    {
        RuleFor(x => x.UserIdentity.KeycloakId)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty();

        RuleFor(x => x.Address!.Street)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Address!.Apartment)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Address!.City)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Address!.Zip)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Address!.Country)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Address!.Region)
            .MaximumLength(50);
    }
}
