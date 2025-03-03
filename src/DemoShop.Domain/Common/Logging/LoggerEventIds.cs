#region

using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Domain.Common.Logging;

public static class LoggerEventIds
{
    public static readonly EventId ApplicationStartup = new(1000, "ApplicationStartup");
    public static readonly EventId ApplicationShutdown = new(1001, "ApplicationShutdown");
    public static readonly EventId ConfigurationLoaded = new(1002, "ConfigurationLoaded");

    // Authentication/Authorization: 2000-2999
    public static readonly EventId AuthSuccess = new(2000, "AuthenticationSuccess");
    public static readonly EventId AuthenticationFailed = new(2001, "AuthenticationFailed");
    public static readonly EventId AuthorizationDenied = new(2002, "AuthorizationDenied");

    // API/HTTP operations: 3000-3999
    public static readonly EventId HttpRequestReceived = new(3000, "HttpRequestReceived");
    public static readonly EventId HttpResponseSent = new(3001, "HttpResponseSent");
    public static readonly EventId ApiRateLimited = new(3002, "ApiRateLimited");
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
    public static readonly EventId GetCurrentUserRequestStarted = new(3400, "GetCurrentUserRequestStarted");
    public static readonly EventId GetCurrentUserRequestSuccess = new(3401, "GetCurrentUserRequestSuccess");
    public static readonly EventId GetCurrentUserRequestFailed = new(3402, "GetCurrentUserRequestSuccess");
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
    public static readonly EventId UserResolutionProcessStarted = new(4400, "UserResolutionProcessStarted");
    public static readonly EventId UserResolutionProcessSuccess = new(4401, "UserResolutionProcessSuccess");
    public static readonly EventId UserResolutionProcessFailed = new(4402, "UserResolutionProcessFailed");
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
    public static readonly EventId OrderItemDeletedDomainEvent = new(5103, "OrderItemDeletedDomainEvent");
    public static readonly EventId OrderItemRestoredDomainEvent = new(5104, "OrderItemRestoredDomainEvent");
    public static readonly EventId ProductCreatedDomainEvent = new(5200, "ProductCreatedDomainEvent");
    public static readonly EventId ProductDeletedDomainEvent = new(5201, "ProductDeletedDomainEvent");
    public static readonly EventId ProductRestoredDomainEvent = new(5202, "ProductRestoredDomainEvent");
    public static readonly EventId ProductImageCreatedDomainEvent = new(5203, "ProductImageCreatedDomainEvent");
    public static readonly EventId ProductImageDeletedDomainEvent = new(5204, "ProductImageDeletedDomainEvent");
    public static readonly EventId ProductImageRestoredDomainEvent = new(5205, "ProductImageRestoredDomainEvent");
    public static readonly EventId ProductCategoryCreatedDomainEvent = new(5206, "ProductCategoryCreatedDomainEvent");
    public static readonly EventId ProductCategoryDeletedDomainEvent = new(5207, "ProductCategoryDeletedDomainEvent");
    public static readonly EventId ProductCategoryRestoredDomainEvent = new(5208, "ProductCategoryRestoredDomainEvent");
    public static readonly EventId UserCreatedDomainEvent = new(5400, "UserCreatedDomainEvent");
    public static readonly EventId UserDeletedDomainEvent = new(5401, "UserDeletedDomainEvent");
    public static readonly EventId UserRestoredDomainEvent = new(5402, "UserRestoredDomainEvent");
    public static readonly EventId UserAddressUpdatedDomainEvent = new(5403, "UserAddressUpdatedDomainEvent");
    public static readonly EventId UserPhoneUpdatedDomainEvent = new(5404, "UserPhoneUpdatedDomainEvent");


    // Domain exceptions: 6000-6999
    public static readonly EventId ValidationStarted = new(6000, "ValidationStarted");
    public static readonly EventId ValidationSuccess = new(6001, "ValidationSuccess");
    public static readonly EventId ValidationFailed = new(6002, "ValidationFailed");
    public static readonly EventId GetOrderByIdDomainException = new(6100, "GetProductByIdDomainException");
    public static readonly EventId GetAllOrdersOfUserDomainException = new(6101, "GetAllOrdersOfUserDomainException");
    public static readonly EventId CreateOrderDomainException = new(6102, "CreateOrderDomainException");
    public static readonly EventId GetAllProductsDomainException = new(6200, "GetAllProductsDomainException");
    public static readonly EventId GetProductByIdDomainException = new(6201, "GetProductByIdDomainException");
    public static readonly EventId GetUserByKeycloakIdDomainException = new(6100, "GetUserByKeycloakIdDomainException");
    public static readonly EventId CreateUserDomainException = new(6101, "CreateUserDomainException");
    public static readonly EventId UpdateUserDomainException = new(6102, "UpdateUserDomainException");

    // Data access: 6000-6999
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
    public static readonly EventId CurrentUserAccessorSuccess = new(7400, "CurrentUserAccessorSuccess");
    public static readonly EventId CurrentUserAccessorNotFound = new(7401, "CurrentUserAccessorNotFound");
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

    // External services: 7000-7999
    public static readonly EventId ExternalServiceCalled = new(8000, "ExternalServiceCalled");
    public static readonly EventId ExternalServiceFailed = new(8001, "ExternalServiceFailed");
}
