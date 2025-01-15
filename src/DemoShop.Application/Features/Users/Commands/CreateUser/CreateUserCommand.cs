using Ardalis.Result;
using DemoShop.Domain.Users.Entities;
using MediatR;

namespace DemoShop.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    Guid KeycloakUserId,
    string Email,
    string Firstname,
    string Lastname
) : IRequest<Result<User>?>;
