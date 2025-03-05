#region

using System.Threading.RateLimiting;
using DemoShop.Domain.Common.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var rateLimitingConfig = configuration.GetSection("RateLimiting");

        if (!rateLimitingConfig.GetValue("EnableRateLimiting", true))
            return;

        services.AddRateLimiter(options =>
        {
            var policyConfig = rateLimitingConfig.GetSection("Policy");

            options.AddPolicy("api", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?.ToString() ??
                httpContext.Request.Headers.Host.ToString(),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policyConfig.GetValue("PermitLimit", 100),
                    Window = policyConfig.GetValue("Window", TimeSpan.FromMinutes(1))
                }
            ));

            // Add event handlers for rate limit events using Serilog
            options.OnRejected = async (context, token) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
                var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var endpoint = context.HttpContext.Request.Method + " " + context.HttpContext.Request.Path.ToString();

                LogRateLimitExceeded(logger, ipAddress, endpoint);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };
        });
    }

    private static void LogRateLimitExceeded(ILogger logger, string ip, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.ApiRateLimited)
            .Warning("Rate limit exceeded for IP: {IpAddress}. Request: {Endpoint}", ip, endpoint);
}
