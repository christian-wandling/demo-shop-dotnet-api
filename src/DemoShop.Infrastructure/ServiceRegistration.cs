using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.Domain.Session.Interfaces;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using DemoShop.Infrastructure.Features.Orders;
using DemoShop.Infrastructure.Features.Products.Persistence;
using DemoShop.Infrastructure.Features.Sessions;
using DemoShop.Infrastructure.Features.Users;
using DemoShop.Infrastructure.Features.Users.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoShop.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IShoppingSessionRepository, SessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.Decorate<IUserRepository, LoggingUserRepositoryDecorator>();

        return services;
    }
}
