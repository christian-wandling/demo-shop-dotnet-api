using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed record UpdateUserAddressCommand(IUpdateUserAddressRequest? UpdateUserAddress)
    : IRequest<Result<AddressResponse?>>;
