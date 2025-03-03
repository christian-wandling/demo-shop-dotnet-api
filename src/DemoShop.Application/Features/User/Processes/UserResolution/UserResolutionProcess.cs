#region

using Ardalis.Result;
using DemoShop.Application.Features.User.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.User.Processes.UserResolution;

public record UserResolutionProcess : IRequest<Result<UserResponse>>;
