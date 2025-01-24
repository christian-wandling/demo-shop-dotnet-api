using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed record UpdateUserPhoneCommand(IUserIdentity UserIdentity, string? NewPhoneNumber) : IRequest<Result<string?>>;
