using Ardalis.Result;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Domain.Users.Entities;
using MediatR;

namespace DemoShop.Application.Features.Users.Queries.GetOrCreateUser;

public record GetOrCreateUserQuery(IUserIdentity Identity) : IRequest<Result<User>>;
