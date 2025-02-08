#region

using Ardalis.Result;
using DemoShop.Application.Features.Product.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.Product.Queries.GetAllProducts;

public sealed record GetAllProductsQuery : IRequest<Result<ProductListResponse>>;
