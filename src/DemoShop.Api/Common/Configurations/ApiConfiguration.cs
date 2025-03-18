#region

using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class ApiConfiguration
{
    public static void ConfigureApi(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddEndpointsApiExplorer();
        services.AddControllers(
            mvcOptions =>
            {
                mvcOptions.Filters.Add(new ProducesAttribute("application/json"));
                mvcOptions.Filters.Add(new ConsumesAttribute("application/json"));
                mvcOptions.AddDefaultResultConvention();
            });
        services.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
            .AddMvc()
            .AddApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

        services.AddHttpContextAccessor();

        services.AddHealthChecks();
    }
}
