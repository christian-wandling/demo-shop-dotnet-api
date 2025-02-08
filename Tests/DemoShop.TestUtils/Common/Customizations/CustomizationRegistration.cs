#region

using DemoShop.TestUtils.Features.Order.Customizations;
using DemoShop.TestUtils.Features.Product.Customizations;
using DemoShop.TestUtils.Features.ShoppingSession;
using DemoShop.TestUtils.Features.User.Customizations;

#endregion

namespace DemoShop.TestUtils.Common.Customizations;

public class CustomizationRegistration : CompositeCustomization
{
    public CustomizationRegistration()
        : base(
            new IntCustomization(),
            new DecimalCustomization(),
            new PriceCustomization(),
            new QuantityCustomization(),
            new UserIdentityCustomization(),
            new CreateAddressDtoCustomization(),
            new AddressEntityCustomization(),
            new UserEntityCustomization(),
            new UpdateUserAddressCommandCustomization(),
            new UpdateUserPhoneCommandCustomization(),
            new UpdateAddressDtoCustomization(),
            new CreateImageDtoCustomization(),
            new ImageEntityCustomization(),
            new CategoryEntityCustomization(),
            new ProductEntityCustomization(),
            new CartItemEntityCustomization(),
            new ShoppingSessionEntityCustomization(),
            new OrderProductCustomization(),
            new OrderItemEntityCustomization(),
            new OrderEntityCustomization(),
            new CreateUserCommandCustomization()
        )
    {
    }
}
