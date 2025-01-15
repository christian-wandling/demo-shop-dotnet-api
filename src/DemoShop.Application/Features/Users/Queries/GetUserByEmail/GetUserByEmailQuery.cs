using Ardalis.Result;
using DemoShop.Domain.Users.Entities;
using MediatR;

namespace DemoShop.Application.Features.Users.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<User>?>;
