using System.Globalization;
using DemoShop.Api.Common.Constants;
using Serilog;
using Serilog.Events;

namespace DemoShop.Api.Common.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration AddEventIdBasedLogging(this LoggerConfiguration logConfig)
    {
        ArgumentNullException.ThrowIfNull(logConfig);

        return LogCategoryConfig.Categories.Aggregate(logConfig, (config, category) =>
            config.WriteTo.Logger(lc =>
                ConfigureFilteredLogger(
                    lc,
                    evt => IsInEventIdRange(evt, category.MinId, category.MaxId),
                    category.FilePath
                )
            )
        );
    }

    public static LoggerConfiguration ConfigureSentryLogging(
        this LoggerConfiguration logConfig,
        IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(logConfig);
        ArgumentNullException.ThrowIfNull(builder);

        var sentryDsn = builder.Configuration["Sentry:Dsn"];
        if (string.IsNullOrWhiteSpace(sentryDsn))
            return logConfig;

        return logConfig.WriteTo.Sentry(options =>
        {
            options.Dsn = sentryDsn;
            options.Debug = false;
            options.AutoSessionTracking = true;
            options.IsGlobalModeEnabled = false;
            options.TracesSampleRate = builder.Environment.IsProduction() ? 0.5 : 1.0;
            options.MinimumBreadcrumbLevel = LogEventLevel.Debug;
            options.MinimumEventLevel = LogEventLevel.Warning;
            options.CaptureFailedRequests = true;
        });
    }

    private static void ConfigureFilteredLogger(
        LoggerConfiguration loggerConfig,
        Func<LogEvent, bool> filterPredicate,
        string filePath
    ) => loggerConfig
        .Filter.ByIncludingOnly(filterPredicate)
        .WriteTo.File(
            filePath,
            outputTemplate: LogOutputConfig.Template,
            formatProvider: CultureInfo.InvariantCulture,
            buffered: true,
            flushToDiskInterval: TimeSpan.FromSeconds(2),
            rollingInterval: RollingInterval.Day
        );

    private static bool IsInEventIdRange(LogEvent logEvent, int minEventId, int maxEventId)
    {
        var eventId = ExtractEventId(logEvent);
        return eventId >= minEventId && eventId.Value <= maxEventId;
    }

    private static int? ExtractEventId(LogEvent logEvent)
    {
        if (!logEvent.Properties.TryGetValue("EventId", out var idProperty))
            return null;

        var idString = idProperty.ToString();
        return int.TryParse(idString, out var id) ? id : null;
    }
}
