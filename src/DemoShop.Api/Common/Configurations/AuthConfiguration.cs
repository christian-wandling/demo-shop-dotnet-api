#region

using Keycloak.AuthServices.Authentication;

#endregion

namespace DemoShop.Api.Common.Configurations;

public static class AuthConfiguration
{
    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddKeycloakWebApiAuthentication(configuration);

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireBuyProductsRole", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "realm_access" &&
                        c.Value.Contains("buy_products", StringComparison.OrdinalIgnoreCase)
                    )
                )
            );
    }
}
