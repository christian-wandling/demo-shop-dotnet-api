using Ardalis.Result;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<Domain.User.Entities.UserEntity>?>;
