using Ardalis.Result;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Domain.Product.Entities;
using MediatR;

namespace DemoShop.Application.Features.Product.Queries.GetAllProducts;

public sealed record GetAllProductsQuery() : IRequest<Result<ProductListResponse>>;
