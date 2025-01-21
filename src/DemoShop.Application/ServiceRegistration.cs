using System.Reflection;
using DemoShop.Application.Features.Common.Events;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.Events;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DemoShop.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IHandle<UserCreatedDomainEvent>, UserCreatedHandler>();
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserValidator>();

        return services;
    }
}
