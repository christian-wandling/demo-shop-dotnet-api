using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed record CreateUserCommand(IUserIdentity UserIdentity) : IRequest<Result<UserEntity>>;
