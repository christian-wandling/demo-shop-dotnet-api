using Microsoft.Extensions.Logging;

namespace DemoShop.Domain.Common.Logging;

public static class LoggerEventIds
{
    private const int CommonApp = 200;
    private const int CommonApi = 600;
    private const int UserApp = 1200;
    private const int UserInfra = 1400;
    private const int UserApi = 1600;

    public static readonly EventId AppIdentityUnauthenticated =
        new(CommonApp + 100, "App:Common:Identity:Unauthenticated");

    public static readonly EventId AppIdentityClaimsExtracted =
        new(CommonApp + 101, "App:Common:Identity:ClaimsExtracted");

    public static readonly EventId AppIdentityClaimsInvalid = new(CommonApp + 102, "App:Common:Identity:ClaimsInvalid");
    public static readonly EventId AppIdentityCreated = new(CommonApp + 103, "App:Common:Identity:Created");

    public static readonly EventId ApiResultError = new(CommonApi + 1, "Api:Common:Result:Error");
    public static readonly EventId ApiResultSuccess = new(CommonApi + 2, "Api:Common:Result:Success");

    public static readonly EventId AppUserCreateStarted = new(UserApp, "App:User:Create:Started");
    public static readonly EventId AppUserCreateValidationFailed = new(UserApp + 1, "App:User:CreateValidation:Failed");
    public static readonly EventId AppUserCreateFailed = new(UserApp + 2, "App:User:Create:Failed");
    public static readonly EventId AppUserCreated = new(UserApp + 3, "App:User:Created");
    public static readonly EventId AppUserGetOrCreateStarted = new(UserApp + 4, "App:User:GetOrCreate:Started");
    public static readonly EventId AppUserFound = new(UserApp + 5, "App:User:Found");
    public static readonly EventId AppUserNotFound = new(UserApp + 6, "App:User:NotFound");
    public static readonly EventId AppUserDeleted = new(UserApp + 7, "App:User:Deleted");
    public static readonly EventId AppUserRestored = new(UserApp + 8, "App:User:Restored");
    public static readonly EventId AppUserAddressUpdated = new(UserApp + 9, "App:User:Address:Updated");
    public static readonly EventId AppUserPhoneUpdated = new(UserApp + 10, "App:User:Phone:Updated");

    public static readonly EventId InfraGetUserByIdStarted = new(UserInfra, "Infra:User:GetUserById:Started");
    public static readonly EventId InfraGetUserByIdSuccess = new(UserInfra + 1, "Infra:User:GetUserById:Success");
    public static readonly EventId InfraGetUserByIdFailed = new(UserInfra + 2, "Infra:User:GetUserById:Failed");
    public static readonly EventId InfraGetUserByEmailStarted = new(UserInfra + 3, "Infra:User:GetUserByEmail:Started");
    public static readonly EventId InfraGetUserByEmailSuccess = new(UserInfra + 4, "Infra:User:GetUserByEmail:Success");
    public static readonly EventId InfraGetUserByEmailFailed = new(UserInfra + 5, "Infra:User:GetUserByEmail:Failed");
    public static readonly EventId InfraCreateUserStarted = new(UserInfra + 6, "Infra:User:CreateUser:Started");
    public static readonly EventId InfraCreateUserSuccess = new(UserInfra + 7, "Infra:User:CreateUser:Success");
    public static readonly EventId InfraCreateUserFailed = new(UserInfra + 8, "Infra:User:CreateUser:Failed");

    public static readonly EventId ApiGetCurrentUserStarted = new(UserApi, "Api:User:GetCurrentUser:Started");
    public static readonly EventId ApiUserIdentityFailed = new(UserApi + 1, "Api:User:UserIdentity:Failed");
}
