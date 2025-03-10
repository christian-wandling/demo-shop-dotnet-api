#region

using DemoShop.Api.Common.Configurations;
using Serilog;

#endregion

namespace DemoShop.Api.Common.Extensions;

public static class BuilderExtensions
{
    public static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();

        builder.Host.UseSerilog();
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.ConfigureApp(builder.Configuration);
        builder.Services.ConfigureAuth(builder.Configuration);
        builder.Services.ConfigureCors(builder.Configuration);
        builder.Services.ConfigureApi(builder.Configuration);
        builder.Services.ConfigureRateLimiting(builder.Configuration);
        builder.Services.ConfigureSwagger();
    }
}
