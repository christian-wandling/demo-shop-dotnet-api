using Ardalis.Result;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<UserEntity>>;
