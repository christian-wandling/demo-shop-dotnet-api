#region

using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.User.Processes.ResolveUser;

public record ResolveUserProcess : IRequest<Result<UserResponse>>;
