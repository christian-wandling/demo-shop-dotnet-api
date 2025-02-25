#region

using System.Globalization;
using Serilog;
using Serilog.Events;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}",
                formatProvider: CultureInfo.InvariantCulture
            )
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)
                .WriteTo.File("logs/errors-.txt",
                    formatProvider: CultureInfo.InvariantCulture,
                    rollingInterval: RollingInterval.Day))
            .WriteTo.Sentry(o =>
            {
                o.Dsn = builder.Configuration["Sentry:Dsn"];
                o.Debug = false;
                o.AutoSessionTracking = true;
                o.IsGlobalModeEnabled = false;
                o.TracesSampleRate = builder.Environment.IsProduction() ? 0.5 : 1.0;
                o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                o.MinimumEventLevel = LogEventLevel.Warning;
                o.CaptureFailedRequests = true;
            })
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }
}
