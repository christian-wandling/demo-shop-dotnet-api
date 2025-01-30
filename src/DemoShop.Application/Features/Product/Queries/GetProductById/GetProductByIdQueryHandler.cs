using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Product.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetUserByKeycloakIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse?>>
{
    public async Task<Result<ProductResponse?>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.Id, nameof(request.Id));

        var product = await repository.GetProductByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (product is not null)
            return Result<ProductResponse?>.Success(mapper.Map<ProductResponse?>(product));

        logger.LogOperationFailed("Get Product By Id", "Id", $"{request.Id}", null);
        return Result.NotFound();
    }
}
