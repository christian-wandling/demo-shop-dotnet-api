using Microsoft.Extensions.Logging;

namespace DemoShop.Domain.Common.Logging;

public static class LoggerEventIds
{
    private const int CommonApi = 0;
    private const int CommonApp = 200;
    private const int UserApi = 1000;
    private const int UserApp = 1200;
    private const int UserInfra = 1200;

    public static readonly EventId ResultError = new(CommonApi + 1, "ResultError");
    public static readonly EventId ResultSuccess = new(CommonApi + 2, "ResultSuccess");

    public static readonly EventId IdentityUnauthenticated = new(CommonApp + 100, "Identity.Unauthenticated");
    public static readonly EventId IdentityClaimsExtracted = new(CommonApp + 101, "Identity.ClaimsExtracted");
    public static readonly EventId IdentityClaimsInvalid = new(CommonApp + 102, "Identity.ClaimsInvalid");
    public static readonly EventId IdentityCreated = new(CommonApp + 103, "Identity.Created");

    public static readonly EventId ApiGetCurrentUserStarted = new(UserApi, "ApiGetCurrentUserStarted");
    public static readonly EventId ApiUserIdentityFailed = new(UserApi + 1, "ApiUserIdentityFailed");
    public static readonly EventId AppUserCreateStarted = new(UserApp, "AppUserCreateStarted");
    public static readonly EventId AppUserCreateValidationFailed = new(UserApp + 1, "AppUserCreateValidationFailed");
    public static readonly EventId AppUserCreateFailed = new(UserApp + 2, "AppUserCreateFailed");
    public static readonly EventId AppUserCreated = new(UserApp + 3, "AppUserCreated");
    public static readonly EventId AppUserGetOrCreateStarted = new(UserApp + 4, "AppUserGetOrCreateStarted");
    public static readonly EventId AppUserFound = new(UserApp + 5, "AppUserFound");
    public static readonly EventId AppUserNotFound = new(UserApp + 6, "AppUserNotFound");

    public static readonly EventId GetUserByIdStarted = new(UserInfra, "GetUserByIdStarted");
    public static readonly EventId GetUserByIdSuccess = new(UserInfra + 1, "GetUserByIdSuccess");
    public static readonly EventId GetUserByIdFailed = new(UserInfra + 2, "GetUserByIdFailed");
    public static readonly EventId GetUserByEmailStarted = new(UserInfra + 3, "GetUserByEmailStarted");
    public static readonly EventId GetUserByEmailSuccess = new(UserInfra + 4, "GetUserByEmailSuccess");
    public static readonly EventId GetUserByEmailFailed = new(UserInfra + 5, "GetUserByEmailFailed");
    public static readonly EventId CreateUserStarted = new(UserInfra + 6, "CreateUserStarted");
    public static readonly EventId CreateUserSuccess = new(UserInfra + 7, "CreateUserSuccess");
    public static readonly EventId CreateUserFailed = new(UserInfra + 8, "CreateUserFailed");
}
