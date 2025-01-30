using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed record GetUserByKeycloakIdQuery(string KeycloakId) : IRequest<Result<UserResponse>>;
