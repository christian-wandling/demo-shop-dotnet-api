#region

using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using MediatR;

#endregion

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed record CreateUserCommand(IUserIdentity UserIdentity) : IRequest<Result<UserResponse>>;
