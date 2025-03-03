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
    public static readonly EventId AuthSuccess = new(2000, "AuthSuccess");
    public static readonly EventId AuthFailed = new(2001, "AuthFailed");
    public static readonly EventId AuthorizationDenied = new(2002, "AuthorizationDenied");

    // API/HTTP operations: 3000-3999
    public static readonly EventId HttpRequestReceived = new(3000, "HttpRequestReceived");
    public static readonly EventId HttpResponseSent = new(3001, "HttpResponseSent");
    public static readonly EventId ApiRateLimited = new(3002, "ApiRateLimited");
    public static readonly EventId GetAllProductsRequestStarted = new(3200, "GetAllProductsRequestStarted");
    public static readonly EventId GetAllProductsRequestSuccess = new(3201, "GetAllProductsRequestSuccess");
    public static readonly EventId GetAllProductsRequestFailed = new(3202, "GetAllProductsRequestSuccess");
    public static readonly EventId GetProductByIdRequestStarted = new(3210, "GetProductByIdRequestStarted");
    public static readonly EventId GetProductByIdRequestSuccess = new(3211, "GetProductByIdRequestSuccess");
    public static readonly EventId GetProductByIdRequestFailed = new(3212, "GetProductByIdRequestSuccess");

    // Application commands/queries 4000-4999
    public static readonly EventId GetAllProductsQueryStarted = new(4200, "GetAllProductsQueryStarted");
    public static readonly EventId GetAllProductsQuerySuccess = new(4201, "GetAllProductsQuerySuccess");
    public static readonly EventId GetProductByIdQueryStarted = new(4210, "GetProductByIdQueryStarted");
    public static readonly EventId GetProductByIdQuerySuccess = new(4211, "GetProductByIdQuerySuccess");
    public static readonly EventId GetProductByIdQueryNotFound = new(4212, "GetProductByIdQueryNotFound");

    // Domain events: 5000-5999
    public static readonly EventId OperationSuccess = new(5000, "OperationSuccess");
    public static readonly EventId DomainEvent = new(5010, "DomainEvent");
    public static readonly EventId ProductCreatedDomainEvent = new(5200, "ProductCreatedDomainEvent");
    public static readonly EventId ProductDeletedDomainEvent = new(5201, "ProductDeletedDomainEvent");
    public static readonly EventId ProductRestoredDomainEvent = new(5202, "ProductRestoredDomainEvent");
    public static readonly EventId ProductImageCreatedDomainEvent = new(5203, "ProductImageCreatedDomainEvent");
    public static readonly EventId ProductImageDeletedDomainEvent = new(5204, "ProductImageDeletedDomainEvent");
    public static readonly EventId ProductImageRestoredDomainEvent = new(5205, "ProductImageRestoredDomainEvent");
    public static readonly EventId ProductCategoryCreatedDomainEvent = new(5206, "ProductCategoryCreatedDomainEvent");
    public static readonly EventId ProductCategoryDeletedDomainEvent = new(5207, "ProductCategoryDeletedDomainEvent");
    public static readonly EventId ProductCategoryRestoredDomainEvent = new(5208, "ProductCategoryRestoredDomainEvent");

    // Domain exceptions: 6000-6999
    public static readonly EventId OperationFailed = new(6001, "OperationFailed");
    public static readonly EventId DomainException = new(6002, "DomainException");
    public static readonly EventId ValidationFailed = new(6003, "ValidationFailed");
    public static readonly EventId GetAllProductsDomainException = new(6200, "GetAllProductsDomainException");
    public static readonly EventId GetProductByIdDomainException = new(6201, "GetProductByIdDomainException");


    // Data access: 6000-6999
    public static readonly EventId CacheWrite = new(7000, "CacheHit");
    public static readonly EventId CacheHit = new(7001, "CacheHit");
    public static readonly EventId CacheMiss = new(7002, "CacheMiss");
    public static readonly EventId GetAllProductsStarted = new(7200, "GetAllProductsStarted");
    public static readonly EventId GetAllProductsSuccess = new(7201, "GetAllProductsCompleted");
    public static readonly EventId GetAllProductsDatabaseException = new(7202, "GetAllProductsDatabaseException");
    public static readonly EventId GetProductByIdStarted = new(7210, "GetProductByIdStarted");
    public static readonly EventId GetProductByIdSuccess = new(7211, "GetProductByIdSuccess");
    public static readonly EventId GetProductByIdDatabaseException = new(7212, "GetProductByIdDatabaseException");
    public static readonly EventId GetProductByIdNotFound = new(7213, "GetProductByIdNotFound");

    // External services: 7000-7999
    public static readonly EventId ExternalServiceCalled = new(8000, "ExternalServiceCalled");
    public static readonly EventId ExternalServiceFailed = new(8001, "ExternalServiceFailed");
}
