using Ardalis.Result;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed record GetUserByKeycloakIdQuery(string KeycloakId) : IRequest<Result<UserEntity>>;
