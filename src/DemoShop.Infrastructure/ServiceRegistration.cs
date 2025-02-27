#region

using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Order.Enums;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using DemoShop.Infrastructure.Common.Services;
using DemoShop.Infrastructure.Features.Orders;
using DemoShop.Infrastructure.Features.Products;
using DemoShop.Infrastructure.Features.ShoppingSessions;
using DemoShop.Infrastructure.Features.ShoppingSessions.Services;
using DemoShop.Infrastructure.Features.Users;
using DemoShop.Infrastructure.Features.Users.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

#endregion

namespace DemoShop.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<OrderStatus>("order_status");
        var dataSource = dataSourceBuilder.Build();

        services.AddSingleton(dataSource);
        services.AddSingleton<IDisposable>(dataSource);

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dataSource));
        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUserIdentityAccessor, UserIdentityAccessor>();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<ICurrentShoppingSessionAccessor, CurrentShoppingSessionAccessor>();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IShoppingSessionRepository, ShoppingSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICacheService, MemoryCacheService>();
    }
}
