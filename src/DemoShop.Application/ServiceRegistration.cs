using System.Reflection;
using DemoShop.Application.Features.Common.Events;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UpdateUserPhoneValidator = DemoShop.Application.Features.User.Commands.UpdateUserPhone.UpdateUserPhoneValidator;

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
