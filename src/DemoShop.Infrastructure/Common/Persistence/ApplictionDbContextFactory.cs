using DemoShop.Domain.Order.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DemoShop.Infrastructure.Common.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var srcDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine(srcDirectory, "DemoShop.Api", "appsettings.json"))
            .AddUserSecrets("aspnet-DemoShop.Api-ea462fe3-c84e-49f5-b411-aff07e717c28")
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("LocalConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<OrderStatus>("order_status");
        var dataSource = dataSourceBuilder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(dataSource);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
