namespace DemoShop.Application.Features.Common.Models;

public sealed record PagingInfo
{
    public required int TotalCount { get; init; }
    public required int PageSize { get; init; }
    public required int CurrentPage { get; init; }
}
