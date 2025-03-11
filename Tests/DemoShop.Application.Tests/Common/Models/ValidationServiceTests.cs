#region

using Ardalis.Result;
using DemoShop.Application.Common.Models;
using DemoShop.Domain.Common.Logging;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Models;
using FluentValidation;
using FluentValidation.Results;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Common.Models;

[Trait("Feature", "Common")]
public class ValidationServiceTests : Test
{
    private readonly ILogger _logger;
    private readonly ValidationService _sut;

    public ValidationServiceTests()
    {
        _logger = Mock<ILogger>();
        _sut = new ValidationService(_logger);
    }

    [Fact]
    public async Task ValidateAsync_WhenValidationSucceeds_ReturnsSuccessResult()
    {
        // Arrange
        var request = Create<TestRequest>();
        var validator = Mock<IValidator<TestRequest>>();
        var validationResult = new ValidationResult();

        validator
            .ValidateAsync(request, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        // Act
        var result = await _sut.ValidateAsync(request, validator, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenValidationFails_ReturnsInvalidResult()
    {
        // Arrange
        var request = Create<TestRequest>();
        var validator = Mock<IValidator<TestRequest>>();
        var validationFailures = new[] { new ValidationFailure("Name", "Name is required") };
        var validationResult = new ValidationResult(validationFailures);

        validator
            .ValidateAsync(request, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        // Act
        var result = await _sut.ValidateAsync(request, validator, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(null)]
    public async Task ValidateAsync_WhenRequestIsNull_ThrowsArgumentNullException(TestRequest request)
    {
        // Arrange
        var validator = Mock<IValidator<TestRequest>>();

        // Act
        var act = () => _sut.ValidateAsync(request, validator, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(request));
    }

    [Fact]
    public async Task ValidateAsync_WhenValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var request = Create<TestRequest>();
        IValidator<TestRequest> validator = null!;

        // Act
        var act = () => _sut.ValidateAsync(request, validator, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(validator));
    }
}
