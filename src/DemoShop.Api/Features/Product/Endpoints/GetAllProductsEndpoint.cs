#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.Product.Endpoints;

[ApiVersion("1.0")]
public class GetAllProductsEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<ProductListResponse>>
{
    [TranslateResultToActionResult]
    [HttpGet("api/v{version:apiVersion}/products")]
    [SwaggerOperation(
        Summary = "Get all products",
        Description = "Get all products",
        OperationId = "GetAllProducts",
        Tags = ["Product"])
    ]
    public override async Task<Result<ProductListResponse>>
        HandleAsync(CancellationToken cancellationToken = default) =>
        await mediator.Send(new GetAllProductsQuery(), cancellationToken);
}
