#region

using FluentValidation;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
