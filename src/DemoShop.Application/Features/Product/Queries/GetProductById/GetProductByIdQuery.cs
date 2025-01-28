using Ardalis.Result;
using DemoShop.Domain.Product.Entities;
using MediatR;

namespace DemoShop.Application.Features.Product.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<ProductEntity?>>;
