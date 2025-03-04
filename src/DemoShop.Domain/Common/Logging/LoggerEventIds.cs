#region

using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Domain.Common.Logging;

public static class LoggerEventIds
{
    // App: 1000-1999
    public static readonly EventId ApplicationStartup = new(1000, "ApplicationStartup");
    public static readonly EventId ApplicationShutdown = new(1001, "ApplicationShutdown");
    public static readonly EventId ConfigurationLoaded = new(1002, "ConfigurationLoaded");

    // Authentication/Authorization: 2000-2999
    public static readonly EventId AuthenticationStarted = new(2000, "AuthenticationStarted");
    public static readonly EventId AuthenticationSuccess = new(2001, "AuthenticationSuccess");
    public static readonly EventId AuthenticationFailed = new(2002, "AuthenticationFailed");

    // API/HTTP operations: 3000-3999
    public static readonly EventId ApiRateLimited = new(3000, "ApiRateLimited");
    public static readonly EventId GetAllOrdersOfUserRequestStarted = new(3100, "GetAllOrdersOfUserRequestStarted");
    public static readonly EventId GetAllOrdersOfUserRequestSuccess = new(3101, "GetAllOrdersOfUserRequestSuccess");
    public static readonly EventId GetAllOrdersOfUserRequestFailed = new(3102, "GetAllOrdersOfUserRequestSuccess");
    public static readonly EventId GetOrderByIdRequestStarted = new(3110, "GetOrderByIdRequestStarted");
    public static readonly EventId GetOrderByIdRequestSuccess = new(3111, "GetOrderByIdRequestSuccess");
    public static readonly EventId GetOrderByIdRequestFailed = new(3112, "GetOrderByIdRequestSuccess");
    public static readonly EventId GetAllProductsRequestStarted = new(3200, "GetAllProductsRequestStarted");
    public static readonly EventId GetAllProductsRequestSuccess = new(3201, "GetAllProductsRequestSuccess");
    public static readonly EventId GetAllProductsRequestFailed = new(3202, "GetAllProductsRequestSuccess");
    public static readonly EventId GetProductByIdRequestStarted = new(3210, "GetProductByIdRequestStarted");
    public static readonly EventId GetProductByIdRequestSuccess = new(3211, "GetProductByIdRequestSuccess");
    public static readonly EventId GetProductByIdRequestFailed = new(3212, "GetProductByIdRequestSuccess");
    public static readonly EventId CheckoutRequestStarted = new(3300, "CheckoutRequestStarted");
    public static readonly EventId CheckoutRequestSuccess = new(3301, "CheckoutRequestSuccess");
    public static readonly EventId CheckoutRequestFailed = new(3302, "CheckoutRequestSuccess");

    public static readonly EventId ResolveCurrentShoppingSessionRequestStarted =
        new(3310, "ResolveCurrentShoppingSessionRequestStarted");

    public static readonly EventId ResolveCurrentShoppingSessionRequestSuccess =
        new(3311, "ResolveCurrentShoppingSessionRequestSuccess");

    public static readonly EventId ResolveCurrentShoppingSessionRequestFailed =
        new(3312, "ResolveCurrentShoppingSessionRequestSuccess");

    public static readonly EventId AddCartItemRequestStarted = new(3350, "AddCartItemRequestStarted");
    public static readonly EventId AddCartItemRequestSuccess = new(3351, "AddCartItemRequestSuccess");
    public static readonly EventId AddCartItemRequestFailed = new(3352, "AddCartItemRequestSuccess");

    public static readonly EventId UpdateCartItemQuantityRequestStarted =
        new(3360, "UpdateCartItemQuantityRequestStarted");

    public static readonly EventId UpdateCartItemQuantityRequestSuccess =
        new(3361, "UpdateCartItemQuantityRequestSuccess");

    public static readonly EventId UpdateCartItemQuantityRequestFailed =
        new(3362, "UpdateCartItemQuantityRequestSuccess");

    public static readonly EventId RemoveCartItemRequestStarted = new(3370, "RemoveCartItemRequestStarted");
    public static readonly EventId RemoveCartItemRequestSuccess = new(3371, "RemoveCartItemRequestSuccess");
    public static readonly EventId RemoveCartItemRequestFailed = new(3372, "RemoveCartItemRequestSuccess");
    public static readonly EventId ResolveCurrentUserRequestStarted = new(3400, "ResolveCurrentUserRequestStarted");
    public static readonly EventId ResolveCurrentUserRequestSuccess = new(3401, "ResolveCurrentUserRequestSuccess");
    public static readonly EventId ResolveCurrentUserRequestFailed = new(3402, "ResolveCurrentUserRequestSuccess");
    public static readonly EventId UpdateUserAddressRequestStarted = new(3410, "UpdateUserAddressRequestStarted");
    public static readonly EventId UpdateUserAddressRequestSuccess = new(3411, "UpdateUserAddressRequestSuccess");
    public static readonly EventId UpdateUserAddressRequestFailed = new(3412, "UpdateUserAddressRequestSuccess");
    public static readonly EventId UpdateUserPhoneRequestStarted = new(3420, "UpdateUserPhoneRequestStarted");
    public static readonly EventId UpdateUserPhoneRequestSuccess = new(3421, "UpdateUserPhoneRequestSuccess");
    public static readonly EventId UpdateUserPhoneRequestFailed = new(3422, "UpdateUserPhoneRequestSuccess");

    // Application commands/queries 4000-4999
    public static readonly EventId GetAllOrdersOfUserQueryStarted = new(4100, "GetAllOrdersOfUserQueryStarted");
    public static readonly EventId GetAllOrdersOfUserQuerySuccess = new(4101, "GetAllOrdersOfUserQuerySuccess");
    public static readonly EventId GetOrderByIdQueryStarted = new(4110, "GetOrderByIdQueryStarted");
    public static readonly EventId GetOrderByIdQuerySuccess = new(4111, "GetOrderByIdQuerySuccess");
    public static readonly EventId GetOrderByIdQueryNotFound = new(4112, "GetOrderByIdQueryNotFound");
    public static readonly EventId CreateOrderCommandStarted = new(4120, "CreateOrderCommandStarted");
    public static readonly EventId CreateOrderCommandSuccess = new(4121, "CreateOrderCommandSuccess");
    public static readonly EventId CreateOrderCommandError = new(4122, "CreateOrderCommandError");
    public static readonly EventId GetAllProductsQueryStarted = new(4200, "GetAllProductsQueryStarted");
    public static readonly EventId GetAllProductsQuerySuccess = new(4201, "GetAllProductsQuerySuccess");
    public static readonly EventId GetProductByIdQueryStarted = new(4210, "GetProductByIdQueryStarted");
    public static readonly EventId GetProductByIdQuerySuccess = new(4211, "GetProductByIdQuerySuccess");
    public static readonly EventId GetProductByIdQueryNotFound = new(4212, "GetProductByIdQueryNotFound");
    public static readonly EventId CheckoutProcessStarted = new(4300, "CheckoutProcessStarted");
    public static readonly EventId CheckoutProcessSuccess = new(4301, "CheckoutProcessSuccess");
    public static readonly EventId CheckoutProcessFailed = new(4302, "CheckoutProcessFailed");

    public static readonly EventId ResolveShoppingSessionProcessStarted =
        new(4310, "ResolveShoppingSessionProcessStarted");

    public static readonly EventId ResolveShoppingSessionProcessSuccess =
        new(4311, "ResolveShoppingSessionProcessSuccess");

    public static readonly EventId ResolveShoppingSessionProcessFailed =
        new(4312, "ResolveShoppingSessionProcessFailed");

    public static readonly EventId GetShoppingSessionByUserIdQueryStarted =
        new(4320, "GetShoppingSessionByUserIdQueryStarted");

    public static readonly EventId GetShoppingSessionByUserIdQuerySuccess =
        new(4321, "GetShoppingSessionByUserIdQuerySuccess");

    public static readonly EventId GetShoppingSessionByUserIdQueryNotFound =
        new(4322, "GetShoppingSessionByUserIdQueryNotFound");

    public static readonly EventId CreateShoppingSessionCommandStarted =
        new(4330, "CreateShoppingSessionCommandStarted");

    public static readonly EventId CreateShoppingSessionCommandSuccess =
        new(4331, "CreateShoppingSessionCommandSuccess");

    public static readonly EventId CreateShoppingSessionCommandError = new(4332, "CreateShoppingSessionCommandError");

    public static readonly EventId DeleteShoppingSessionCommandStarted =
        new(4340, "DeleteShoppingSessionCommandStarted");

    public static readonly EventId DeleteShoppingSessionCommandSuccess =
        new(4341, "DeleteShoppingSessionCommandSuccess");

    public static readonly EventId DeleteShoppingSessionCommandError = new(4342, "DeleteShoppingSessionCommandError");

    public static readonly EventId AddCartItemCommandStarted =
        new(4350, "AddCartItemCommandStarted");

    public static readonly EventId AddCartItemCommandSuccess =
        new(4351, "AddCartItemCommandSuccess");

    public static readonly EventId AddCartItemCommandError = new(4352, "AddCartItemCommandError");

    public static readonly EventId UpdateCartItemQuantityCommandStarted =
        new(4360, "UpdateCartItemQuantityCommandStarted");

    public static readonly EventId UpdateCartItemQuantityCommandSuccess =
        new(4361, "UpdateCartItemQuantityCommandSuccess");

    public static readonly EventId UpdateCartItemQuantityCommandError = new(4362, "UpdateCartItemQuantityCommandError");

    public static readonly EventId RemoveCartItemCommandStarted =
        new(4370, "RemoveCartItemCommandStarted");

    public static readonly EventId RemoveCartItemCommandSuccess =
        new(4371, "RemoveCartItemCommandSuccess");

    public static readonly EventId RemoveCartItemCommandError = new(4372, "RemoveCartItemCommandError");

    public static readonly EventId ResolveUserProcessStarted = new(4400, "ResolveUserProcessStarted");
    public static readonly EventId ResolveUserProcessSuccess = new(4401, "ResolveUserProcessSuccess");
    public static readonly EventId ResolveUserProcessFailed = new(4402, "ResolveUserProcessFailed");
    public static readonly EventId GetUserByKeycloakIdQueryStarted = new(4410, "GetUserByKeycloakIdQueryStarted");
    public static readonly EventId GetUserByKeycloakIdQuerySuccess = new(4411, "GetUserByKeycloakIdQuerySuccess");
    public static readonly EventId GetUserByKeycloakIdQueryNotFound = new(4412, "GetUserByKeycloakIdQueryNotFound");
    public static readonly EventId CreateUserCommandStarted = new(4420, "CreateUserCommandStarted");
    public static readonly EventId CreateUserCommandSuccess = new(4421, "CreateUserCommandSuccess");
    public static readonly EventId CreateUserCommandError = new(4422, "CreateUserCommandError");
    public static readonly EventId UpdateUserAddressCommandStarted = new(4430, "UpdateUserAddressCommandStarted");
    public static readonly EventId UpdateUserAddressCommandSuccess = new(4431, "UpdateUserAddressCommandSuccess");
    public static readonly EventId UpdateUserAddressCommandError = new(4432, "UpdateUserAddressCommandError");
    public static readonly EventId UpdateUserPhoneCommandStarted = new(4440, "UpdateUserPhoneCommandStarted");
    public static readonly EventId UpdateUserPhoneCommandSuccess = new(4441, "UpdateUserPhoneCommandSuccess");
    public static readonly EventId UpdateUserPhoneCommandError = new(4442, "UpdateUserPhoneCommandError");

    // Domain events: 5000-5999
    public static readonly EventId OrderCreatedDomainEvent = new(5100, "OrderCreatedDomainEvent");
    public static readonly EventId OrderDeletedDomainEvent = new(5101, "OrderDeletedDomainEvent");
    public static readonly EventId OrderRestoredDomainEvent = new(5102, "OrderRestoredDomainEvent");
    public static readonly EventId ProductCreatedDomainEvent = new(5200, "ProductCreatedDomainEvent");
    public static readonly EventId ProductDeletedDomainEvent = new(5201, "ProductDeletedDomainEvent");
    public static readonly EventId ProductRestoredDomainEvent = new(5202, "ProductRestoredDomainEvent");
    public static readonly EventId ProductImageCreatedDomainEvent = new(5203, "ProductImageCreatedDomainEvent");
    public static readonly EventId ProductImageDeletedDomainEvent = new(5204, "ProductImageDeletedDomainEvent");
    public static readonly EventId ProductImageRestoredDomainEvent = new(5205, "ProductImageRestoredDomainEvent");
    public static readonly EventId ProductCategoryCreatedDomainEvent = new(5206, "ProductCategoryCreatedDomainEvent");
    public static readonly EventId ProductCategoryDeletedDomainEvent = new(5207, "ProductCategoryDeletedDomainEvent");
    public static readonly EventId ProductCategoryRestoredDomainEvent = new(5208, "ProductCategoryRestoredDomainEvent");
    public static readonly EventId ShoppingSessionCreatedDomainEvent = new(5300, "ShoppingSessionCreatedDomainEvent");

    public static readonly EventId ShoppingSessionConvertedDomainEvent =
        new(5301, "ShoppingSessionConvertedDomainEvent");

    public static readonly EventId CartItemAddedDomainEvent = new(5302, "CartItemAddedDomainEvent");
    public static readonly EventId CartItemQuantityChangedDomainEvent = new(5303, "CartItemQuantityChangedDomainEvent");
    public static readonly EventId CartItemRemovedDomainEvent = new(5304, "CartItemRemovedDomainEvent");
    public static readonly EventId UserCreatedDomainEvent = new(5400, "UserCreatedDomainEvent");
    public static readonly EventId UserDeletedDomainEvent = new(5401, "UserDeletedDomainEvent");
    public static readonly EventId UserRestoredDomainEvent = new(5402, "UserRestoredDomainEvent");
    public static readonly EventId UserAddressUpdatedDomainEvent = new(5403, "UserAddressUpdatedDomainEvent");
    public static readonly EventId UserPhoneUpdatedDomainEvent = new(5404, "UserPhoneUpdatedDomainEvent");

    // Domain exceptions: 6000-6999
    public static readonly EventId UnhandledException = new(6000, "UnhandledException");
    public static readonly EventId UnhandledDomainException = new(6001, "UnhandledDomainException");
    public static readonly EventId UnhandledValidationException = new(6002, "UnhandledValidationException");
    public static readonly EventId UnhandledAuthException = new(6003, "UnhandledAuthException");
    public static readonly EventId UnhandledDbException = new(6004, "UnhandledDbException");
    public static readonly EventId ValidationStarted = new(6010, "ValidationStarted");
    public static readonly EventId ValidationSuccess = new(6011, "ValidationSuccess");
    public static readonly EventId ValidationFailed = new(6012, "ValidationFailed");
    public static readonly EventId GetOrderByIdDomainException = new(6100, "GetProductByIdDomainException");
    public static readonly EventId GetAllOrdersOfUserDomainException = new(6101, "GetAllOrdersOfUserDomainException");
    public static readonly EventId CreateOrderDomainException = new(6102, "CreateOrderDomainException");
    public static readonly EventId GetAllProductsDomainException = new(6200, "GetAllProductsDomainException");
    public static readonly EventId GetProductByIdDomainException = new(6201, "GetProductByIdDomainException");

    public static readonly EventId GetShoppingSessionByUserIdDomainException =
        new(6300, "GetShoppingSessionByUserIdDomainException");

    public static readonly EventId CreateShoppingSessionDomainException =
        new(6301, "CreateShoppingSessionDomainException");

    public static readonly EventId UpdateShoppingSessionDomainException =
        new(6302, "UpdateShoppingSessionDomainException");

    public static readonly EventId DeleteShoppingSessionDomainException =
        new(6303, "DeleteShoppingSessionDomainException");

    public static readonly EventId GetUserByKeycloakIdDomainException = new(6400, "GetUserByKeycloakIdDomainException");
    public static readonly EventId CreateUserDomainException = new(6401, "CreateUserDomainException");
    public static readonly EventId UpdateUserDomainException = new(6402, "UpdateUserDomainException");

    // Data access: 7000-7999
    public static readonly EventId CacheWrite = new(7000, "CacheHit");
    public static readonly EventId CacheHit = new(7001, "CacheHit");
    public static readonly EventId CacheMiss = new(7002, "CacheMiss");
    public static readonly EventId CacheInvalidate = new(7003, "CacheInvalidate");
    public static readonly EventId TransactionStarted = new(7010, "TransactionStarted");
    public static readonly EventId TransactionSuccess = new(7011, "TransactionSuccess");
    public static readonly EventId TransactionRollback = new(7012, "TransactionRollback");
    public static readonly EventId TransactionDisposed = new(7014, "TransactionDisposed");
    public static readonly EventId GetOrdersByUserIdStarted = new(7100, "GetOrdersByUserIdStarted");
    public static readonly EventId GetOrdersByUserIdSuccess = new(7101, "GetOrdersByUserIdCompleted");
    public static readonly EventId GetOrdersByUserIdDatabaseException = new(7102, "GetOrdersByUserIdDatabaseException");
    public static readonly EventId GetOrderByIdStarted = new(7210, "GetOrderByIdStarted");
    public static readonly EventId GetOrderByIdSuccess = new(7211, "GetOrderByIdSuccess");
    public static readonly EventId GetOrderByIdDatabaseException = new(7212, "GetOrderByIdDatabaseException");
    public static readonly EventId GetOrderByIdNotFound = new(7113, "GetOrderByIdNotFound");
    public static readonly EventId CreateOrderStarted = new(7220, "CreateOrderStarted");
    public static readonly EventId CreateOrderSuccess = new(7221, "CreateOrderSuccess");
    public static readonly EventId CreateOrderDatabaseException = new(7222, "CreateOrderDatabaseException");
    public static readonly EventId GetAllProductsStarted = new(7200, "GetAllProductsStarted");
    public static readonly EventId GetAllProductsSuccess = new(7201, "GetAllProductsCompleted");
    public static readonly EventId GetAllProductsDatabaseException = new(7202, "GetAllProductsDatabaseException");
    public static readonly EventId GetProductByIdStarted = new(7210, "GetProductByIdStarted");
    public static readonly EventId GetProductByIdSuccess = new(7211, "GetProductByIdSuccess");
    public static readonly EventId GetProductByIdDatabaseException = new(7212, "GetProductByIdDatabaseException");
    public static readonly EventId GetProductByIdNotFound = new(7213, "GetProductByIdNotFound");

    public static readonly EventId CurrentShoppingSessionAccessorStarted =
        new(7300, "CurrentShoppingSessionAccessorStarted");

    public static readonly EventId CurrentShoppingSessionAccessorSuccess =
        new(7301, "CurrentShoppingSessionAccessorSuccess");

    public static readonly EventId CurrentShoppingSessionAccessorNotFound =
        new(7302, "CurrentShoppingSessionAccessorNotFound");

    public static readonly EventId GetShoppingSessionByUserIdStarted = new(7310, "GetShoppingSessionByUserIdStarted");
    public static readonly EventId GetShoppingSessionByUserIdSuccess = new(7311, "GetShoppingSessionByUserIdSuccess");

    public static readonly EventId GetShoppingSessionByUserIdDatabaseException =
        new(7312, "GetShoppingSessionByUserIdDatabaseException");

    public static readonly EventId GetShoppingSessionByUserIdNotFound = new(7313, "GetShoppingSessionByUserIdNotFound");
    public static readonly EventId CreateShoppingSessionStarted = new(7320, "CreateShoppingSessionStarted");
    public static readonly EventId CreateShoppingSessionSuccess = new(7321, "CreateShoppingSessionSuccess");

    public static readonly EventId CreateShoppingSessionDatabaseException =
        new(7322, "CreateShoppingSessionDatabaseException");

    public static readonly EventId UpdateShoppingSessionStarted = new(7330, "UpdateShoppingSessionStarted");
    public static readonly EventId UpdateShoppingSessionSuccess = new(7331, "UpdateShoppingSessionSuccess");

    public static readonly EventId UpdateShoppingSessionDatabaseException =
        new(7332, "UpdateShoppingSessionDatabaseException");

    public static readonly EventId DeleteShoppingSessionStarted = new(7340, "DeleteShoppingSessionStarted");
    public static readonly EventId DeleteShoppingSessionSuccess = new(7341, "DeleteShoppingSessionSuccess");
    public static readonly EventId DeleteShoppingSessionFailed = new(7342, "DeleteShoppingSessionFailed");

    public static readonly EventId DeleteShoppingSessionDatabaseException =
        new(7343, "DeleteShoppingSessionDatabaseException");

    public static readonly EventId CurrentUserAccessorStarted = new(7400, "CurrentUserAccessorStarted");
    public static readonly EventId CurrentUserAccessorSuccess = new(7401, "CurrentUserAccessorSuccess");
    public static readonly EventId CurrentUserAccessorNotFound = new(7402, "CurrentUserAccessorNotFound");
    public static readonly EventId GetUserByKeycloakUserIdStarted = new(7410, "GetUserByKeycloakUserIdStarted");
    public static readonly EventId GetUserByKeycloakUserIdSuccess = new(7411, "GetUserByKeycloakUserIdSuccess");

    public static readonly EventId GetUserByKeycloakIdDatabaseException =
        new(7412, "GetUserByKeycloakIdDatabaseException");

    public static readonly EventId GetUserByKeycloakUserIdNotFound = new(7413, "GetUserByKeycloakUserIdNotFound");
    public static readonly EventId CreateUserStarted = new(7420, "CreateUserStarted");
    public static readonly EventId CreateUserSuccess = new(7421, "CreateUserSuccess");
    public static readonly EventId CreateUserDatabaseException = new(7422, "CreateUserDatabaseException");
    public static readonly EventId UpdateUserStarted = new(7430, "UpdateUserStarted");
    public static readonly EventId UpdateUserSuccess = new(7431, "UpdateUserSuccess");
    public static readonly EventId UpdateUserDatabaseException = new(7432, "UpdateUserDatabaseException");
}
