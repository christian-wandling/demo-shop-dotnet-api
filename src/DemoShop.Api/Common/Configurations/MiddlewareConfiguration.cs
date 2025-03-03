using DemoShop.Domain.Common.Logging;

namespace DemoShop.Api.Common.Configurations;

public static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsDevelopment()) app.ConfigureSwagger();

        app.MapControllers();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseCors();
        app.UseRateLimiter();
        app.MapHealthChecks("/health");
    }

    private static void ConfigureSwagger(this WebApplication app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/{documentname}/swagger.json";
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/api/v1/swagger.json", "v1");
            options.RoutePrefix = "api";
        });
    }
}
