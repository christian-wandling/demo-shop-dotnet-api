#region

using System.Globalization;
using DemoShop.Api.Common.Constants;
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

        var loggerConfiguration = new LoggerConfiguration();
        if (builder.Environment.IsDevelopment())
        {
            loggerConfiguration
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information);
        }
        else
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error);
        }

        loggerConfiguration
            .Enrich.WithProperty("Application", "DemoShop")
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: LogOutputConfig.Template, formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.File("logs/errors-.txt",
                restrictedToMinimumLevel: LogEventLevel.Error,
                outputTemplate: LogOutputConfig.Template,
                formatProvider: CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day)
            .AddEventIdBasedLogging()
            .ConfigureSentryLogging(builder);

        Log.Logger = loggerConfiguration.CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);
        builder.Services.AddSingleton(Log.Logger);
    }
}
