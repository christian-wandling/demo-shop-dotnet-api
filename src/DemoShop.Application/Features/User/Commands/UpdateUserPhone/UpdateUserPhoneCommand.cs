using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed record UpdateUserPhoneCommand(IUserIdentity UserIdentity, string? NewPhoneNumber) : IRequest<Result<string?>>;
