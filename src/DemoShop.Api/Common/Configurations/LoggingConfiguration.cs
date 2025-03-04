#region

using System.Globalization;
using DemoShop.Api.Common.Extensions;
using Serilog;
using Serilog.Events;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "DemoShop")
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {EventId} {Message:lj} {NewLine}{Exception}",
                formatProvider: CultureInfo.InvariantCulture
            )
            .WriteTo.File("logs/errors-.txt",
                restrictedToMinimumLevel: LogEventLevel.Error,
                formatProvider: CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day)
            .AddEventIdBasedLogging()
            .ConfigureSentryLogging(builder)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Services.AddSingleton(Log.Logger);
    }
}
