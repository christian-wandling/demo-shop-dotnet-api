using System.Text.RegularExpressions;

namespace DemoShop.Api.Common.Configurations;

public static partial class MiddlewareConfiguration
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
        app.UseSwagger(c => c.RouteTemplate = "api/docs/{documentName}/swagger.json");
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/api/docs/v1/swagger.json", "v1");
            options.RoutePrefix = "api/docs";
        });
        app.Use(async (context, next) =>
        {
            if (MyRegex().IsMatch(context.Request.Path))
            {
                context.Response.Redirect("/api/docs");
                return;
            }

            await next();
        });
    }

    [GeneratedRegex(@"^\/(api\/?)?(index\.html)?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex();
}
