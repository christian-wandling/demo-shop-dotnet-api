#region

using FluentValidation;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;

public sealed class CreateShoppingSessionCommandValidator : AbstractValidator<CreateShoppingSessionCommand>
{
    public CreateShoppingSessionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}
