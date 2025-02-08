#region

using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.Product.Models;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.Product.Endpoints;

[ApiVersion("1.0")]
public class GetProductByIdEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithRequest<GetProductByIdRequest>.WithResult<Result<ProductResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.NotFound)]
    [HttpGet("api/v{version:apiVersion}/products/{id:int}")]
    [SwaggerOperation(
        Summary = "Get all products",
        Description = "Get all products",
        OperationId = "Product.GetProductById",
        Tags = ["Product"])
    ]
    public override async Task<Result<ProductResponse>> HandleAsync(
        [FromRoute] GetProductByIdRequest request,
        CancellationToken cancellationToken = default
    )
    {
        Guard.Against.Null(request, nameof(request));

        return await mediator.Send(new GetProductByIdQuery(request.Id), cancellationToken);
    }
}
