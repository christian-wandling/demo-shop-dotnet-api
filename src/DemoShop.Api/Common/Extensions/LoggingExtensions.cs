using System.Globalization;
using DemoShop.Api.Common.Constants;
using Serilog;
using Serilog.Events;

namespace DemoShop.Api.Common.Extensions;

public static class LoggingConfigurationExtensions
{
    public static LoggerConfiguration AddEventIdBasedLogging(this LoggerConfiguration logConfig)
    {
        ArgumentNullException.ThrowIfNull(logConfig);

        foreach (var (minId, (maxId, filePath)) in LogCategoryConfig.Categories)
        {
            logConfig.WriteTo.Logger(ConfigureLoggerForEventIdRange(minId, maxId, filePath));
        }

        return logConfig;
    }

    public static LoggerConfiguration ConfigureSentryLogging(this LoggerConfiguration logConfig,
        IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(logConfig);
        ArgumentNullException.ThrowIfNull(builder);

        var sentryDsn = builder.Configuration["Sentry:Dsn"];
        if (!string.IsNullOrWhiteSpace(sentryDsn))
        {
            logConfig.WriteTo.Sentry(o =>
            {
                o.Dsn = sentryDsn;
                o.Debug = false;
                o.AutoSessionTracking = true;
                o.IsGlobalModeEnabled = false;
                o.TracesSampleRate = builder.Environment.IsProduction() ? 0.5 : 1.0;
                o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                o.MinimumEventLevel = LogEventLevel.Warning;
                o.CaptureFailedRequests = true;
            });
        }

        return logConfig;
    }

    private static Action<LoggerConfiguration> ConfigureLoggerForEventIdRange(
        int minEventId, int maxEventId, string filePath) => lc =>
        lc.Filter.ByIncludingOnly(evt =>
                evt.Properties.TryGetValue("EventId", out var eventIdProp)
                && eventIdProp is ScalarValue { Value: int id }
                && id >= minEventId
                && id <= maxEventId
            )
            .WriteTo.File(
                filePath,
                formatProvider: CultureInfo.InvariantCulture,
                buffered: true,
                flushToDiskInterval: TimeSpan.FromSeconds(2),
                rollingInterval: RollingInterval.Day
            );
}
