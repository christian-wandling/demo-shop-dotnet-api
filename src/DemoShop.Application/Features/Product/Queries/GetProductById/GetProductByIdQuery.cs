#region

using Ardalis.Result;
using DemoShop.Application.Features.Product.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.Product.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponse>>;
