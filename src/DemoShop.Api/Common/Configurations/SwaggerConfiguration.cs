#region

using Microsoft.OpenApi.Models;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSwaggerGen(
            c =>
            {
                c.SupportNonNullableReferenceTypes();
                c.DescribeAllParametersInCamelCase();
                c.UseOneOfForPolymorphism();
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Demo Shop API",
                        Version = "v1",
                        Description =
                            "A comprehensive API for managing an online store, providing endpoints for product catalog, user management, shopping cart operations, and order processing"
                    });
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme { Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Description = "Enter 'Bearer {token}'" });
                c.EnableAnnotations();
            });
    }
}
