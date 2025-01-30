using FluentValidation;

namespace DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.AddCartItem.ProductId)
            .GreaterThan(0);
    }
}
