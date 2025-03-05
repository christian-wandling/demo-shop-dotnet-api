#region

using System.Text.Json;
using DemoShop.Api;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.TestUtils.Common.Services;
using FluentAssertions;
using Xunit.Abstractions;

#endregion

namespace DemoShop.IntegrationTests.Product;

public class GetAllProductsTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };


    [Fact]
    public async Task GetAllProducts_ReturnsSuccessStatusCode()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsCorrectContentType()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products");

        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetAllProducts_ReturnsNonEmptyProductsList()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products");

        var content = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<ProductListResponse>(content, _jsonOptions);

        products.Should().NotBeNull();
        products.Items.Should().NotBeEmpty();
    }
}
