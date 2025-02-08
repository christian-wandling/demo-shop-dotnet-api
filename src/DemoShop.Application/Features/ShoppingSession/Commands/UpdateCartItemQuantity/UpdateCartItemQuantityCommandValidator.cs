#region

using FluentValidation;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;

public sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.UpdateCartItem.Quantity)
            .GreaterThan(0);
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
