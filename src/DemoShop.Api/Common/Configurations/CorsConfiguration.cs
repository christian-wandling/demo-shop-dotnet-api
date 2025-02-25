namespace DemoShop.Api.Common.Configurations;

public static class CorsConfiguration
{
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(
                                configuration.GetSection("AllowedOrigins").Get<string[]>() ??
                                ["http://localhost:4200"])
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
    }
}
