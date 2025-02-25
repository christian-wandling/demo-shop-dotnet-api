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
        app.MapHealthChecks("/health");
    }

    private static void ConfigureSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }
}
