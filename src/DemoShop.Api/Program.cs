#region

using DemoShop.Api.Common.Configurations;
using DemoShop.Api.Common.Extensions;
using DemoShop.Domain.Common.Logging;
using Serilog;

#endregion

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureBuilder();
    LoggingConfiguration.ConfigureLogging(builder);
    Log.ForContext("EventId", LoggerEventId.ConfigurationLoaded)
        .Debug("Configuration loaded");

    builder.ConfigureServices();

    var app = builder.Build();
    app.ConfigureMiddleware();
    Log.ForContext("EventId", LoggerEventId.ApplicationStartup)
        .Information("Application starting");

    app.Lifetime.ApplicationStopping.Register(() => Log
        .ForContext("EventId", LoggerEventId.ApplicationShutdown)
        .Information("Application shutting down"));

    app.Run();
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031
{
    Log.ForContext("EventId", LoggerEventId.ApplicationStartupFailed)
        .Fatal(ex, "Application startup failed. Reason: {Message}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}

namespace DemoShop.Api
{
    public partial class Program
    {
    }
}
