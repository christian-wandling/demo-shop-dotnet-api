#region

using DemoShop.Api.Common.Middleware;
using DemoShop.Application;
using DemoShop.Infrastructure;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class AppConfiguration
{
    public static void ConfigureApp(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddMemoryCache();

        services.AddApplication();
        services.AddInfrastructure(configuration);
    }
}
