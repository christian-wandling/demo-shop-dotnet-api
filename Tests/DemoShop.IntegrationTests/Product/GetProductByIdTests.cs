#region

using System.Net;
using System.Text.Json;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.TestUtils.Common.Services;
using FluentAssertions;
using Xunit.Abstractions;

#endregion

namespace DemoShop.IntegrationTests.Product;

public class GetProductByIdTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetProductById_ReturnsSuccessStatusCode()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products/1");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetProductById_ReturnsCorrectContentType()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products/1");

        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetProductById_ReturnsNonEmptyResponse()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products/1");

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductResponse>(content, _jsonOptions);

        product.Should().NotBeNull();
        product.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNotFound()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products/999999"); // Assuming this ID doesn't exist

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProductById_WithInvalidIdFormat_ReturnsBadRequest()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/products/-1");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
