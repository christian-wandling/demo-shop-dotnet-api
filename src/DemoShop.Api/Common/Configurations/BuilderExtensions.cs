#region

using Serilog;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class BuilderExtensions
{
    public static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
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
