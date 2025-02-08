#region

using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using MediatR;

#endregion

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed record UpdateUserPhoneCommand(IUpdateUserPhoneRequest UpdateUser)
    : IRequest<Result<UserPhoneResponse>>;
