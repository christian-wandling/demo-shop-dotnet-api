#region

using System.Text.Json;
using DemoShop.Api.Common.Middleware;
using DemoShop.Domain.Common.Base;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Api.Tests.Common;

public class GlobalExceptionHandlerTests : Test
{
    private readonly HttpContext _httpContext;
    private readonly Stream _responseStream;
    private readonly GlobalExceptionHandler _sut;

    public GlobalExceptionHandlerTests()
    {
        var logger = Mock<ILogger<GlobalExceptionHandler>>();
        _sut = new GlobalExceptionHandler(logger);

        // Setup HTTP context mock
        _httpContext = Substitute.For<HttpContext>();
        _responseStream = new MemoryStream();
        var response = Substitute.For<HttpResponse>();
        response.Body = _responseStream;
        _httpContext.Response.Returns(response);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainException_ReturnsProblemDetails()
    {
        // Arrange
        var exception = new DomainException("Domain error message");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        VerifyResponseBody("Domain Error", "Domain error message", StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task TryHandleAsync_WhenValidationException_ReturnsProblemDetails()
    {
        // Arrange
        var exception = new ValidationException("Validation error message");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        VerifyResponseBody("Validation Error", "Validation error message", StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnauthorizedAccessException_ReturnsProblemDetails()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Unauthorized message");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        VerifyResponseBody("Unauthorized", "Unauthorized message", StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task TryHandleAsync_WhenInvalidOperationException_ReturnsProblemDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Invalid operation message");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        VerifyResponseBody("Invalid Operation", "Invalid operation message", StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task TryHandleAsync_WhenArgumentException_ReturnsProblemDetails()
    {
        // Arrange
        var exception = new ArgumentException("Bad request message");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        VerifyResponseBody("Bad Request", "Bad request message", StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnhandledException_ReturnsTrue()
    {
        // Arrange
        var exception = new Exception("Unhandled exception");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TryHandleAsync_WhenNullException_ThrowsArgumentNullException() =>
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.TryHandleAsync(_httpContext, null!, CancellationToken.None).AsTask());

    [Fact]
    public async Task TryHandleAsync_WhenNullHttpContext_ThrowsArgumentNullException()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.TryHandleAsync(null!, exception, CancellationToken.None).AsTask());
    }

    private void VerifyResponseBody(string expectedTitle, string expectedDetail, int expectedStatus)
    {
        _responseStream.Position = 0;
        using var reader = new StreamReader(_responseStream);
        var responseBody = reader.ReadToEnd();

        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);

        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be(expectedTitle);
        problemDetails.Detail.Should().Be(expectedDetail);
        problemDetails.Status.Should().Be(expectedStatus);
        problemDetails.Type.Should().Be($"https://tools.ietf.org/html/rfc7231#section-6.5.{expectedStatus - 399}");
    }
}
