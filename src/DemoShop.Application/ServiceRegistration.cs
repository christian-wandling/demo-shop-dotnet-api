#region

using System.Reflection;
using DemoShop.Application.Common.Events;
using DemoShop.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace DemoShop.Application;

public static class ServiceRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
