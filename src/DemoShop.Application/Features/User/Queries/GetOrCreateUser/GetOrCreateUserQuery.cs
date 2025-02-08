#region

using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public record GetOrCreateUserQuery : IRequest<Result<UserResponse>>;
