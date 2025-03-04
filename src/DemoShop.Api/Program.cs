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
    Log.Information("Configuration loaded {@EventId}", LoggerEventIds.ConfigurationLoaded);
    builder.ConfigureServices();
    var app = builder.Build();
    app.ConfigureMiddleware();
    app.Lifetime.ApplicationStopping.Register(() =>
        Log.Information("Application shutting down {@EventId}", LoggerEventIds.ApplicationShutdown));
    Log.Information("Application starting {@EventId}", LoggerEventIds.ApplicationStartup);

    app.Run();
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}
