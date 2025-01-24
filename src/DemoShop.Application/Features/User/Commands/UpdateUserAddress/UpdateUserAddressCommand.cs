using Ardalis.Result;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed record UpdateUserAddressCommand(IUserIdentity UserIdentity, IUpdateUserAddressRequest? Address)
    : IRequest<Result<AddressEntity?>>;
