#region

using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed record GetUserByKeycloakIdQuery(string KeycloakId) : IRequest<Result<UserResponse>>;
