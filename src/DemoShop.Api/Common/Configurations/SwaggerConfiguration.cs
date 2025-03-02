#region

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo Shop API", Version = "v1" });
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer" });
                c.EnableAnnotations();
            });
    }
}
