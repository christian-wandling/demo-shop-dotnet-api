using System.Reflection;
using DemoShop.Application.Features.Common.Events;
using DemoShop.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DemoShop.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
