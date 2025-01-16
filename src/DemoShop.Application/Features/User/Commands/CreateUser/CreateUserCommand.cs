using Ardalis.Result;
using MediatR;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed record CreateUserCommand(
    Guid KeycloakUserId,
    string Email,
    string Firstname,
    string Lastname
) : IRequest<Result<Domain.User.Entities.UserEntity>?>;
