using Ardalis.Result;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public record GetOrCreateUserQuery(IUserIdentity Identity) : IRequest<Result<UserEntity>>;
