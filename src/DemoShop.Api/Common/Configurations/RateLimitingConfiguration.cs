using System.Threading.RateLimiting;

namespace DemoShop.Api.Common.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var rateLimitingConfig = configuration.GetSection("RateLimiting");

        if (!rateLimitingConfig.GetValue<bool>("EnableRateLimiting", true))
            return;

        services.AddRateLimiter(options =>
        {
            var policyConfig = rateLimitingConfig.GetSection("Policy");

            options.AddPolicy("api", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ??
                              httpContext.Request.Headers.Host.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policyConfig.GetValue("PermitLimit", 100),
                    Window = policyConfig.GetValue("Window", TimeSpan.FromMinutes(1)),
                }
            ));
        });
    }
}
