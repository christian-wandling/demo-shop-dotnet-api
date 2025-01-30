using Ardalis.Result;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Domain.Product.Entities;
using MediatR;

namespace DemoShop.Application.Features.Product.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponse?>>;
