#region

using Ardalis.Result;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.Entities;

[Trait("Feature", "User")]
public class AddressEntityTests : Test
{
    public class Create : AddressEntityTests
    {
        [Fact]
        public void Create_Successfully()
        {
            // Arrange
            var dto = Fixture.Build<CreateAddressDto>()
                .With(x => x.UserId, 1)
                .With(x => x.Street, "Test Street")
                .With(x => x.Apartment, "Apt 1")
                .With(x => x.City, "Test City")
                .With(x => x.Zip, "12345")
                .With(x => x.Country, "Test Country")
                .Create();

            // Act
            var result = AddressEntity.Create(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Street.Should().Be(dto.Street);
            result.Value.Apartment.Should().Be(dto.Apartment);
            result.Value.City.Should().Be(dto.City);
            result.Value.Zip.Should().Be(dto.Zip);
            result.Value.Country.Should().Be(dto.Country);
            result.Value.Region.Should().Be(dto.Region);
            result.Value.UserId.Should().Be(dto.UserId);
        }

        [Fact]
        public void Fail_WithNullDto()
        {
            // Act & Assert
            var action = () => AddressEntity.Create(null!);

            action.Should().Throw<ArgumentNullException>()
                .WithParameterName("createAddress");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Fail_WithInvalidUserId(int userId)
        {
            // Arrange
            var dto = Fixture.Build<CreateAddressDto>()
                .With(x => x.UserId, userId)
                .Create();

            // Act & Assert
            var action = () => AddressEntity.Create(dto);

            action.Should().Throw<ArgumentException>();
        }
    }

    public class Update : AddressEntityTests
    {
        [Fact]
        public void Update_Successfully()
        {
            // Arrange
            var address = AddressEntity.Create(Create<CreateAddressDto>()).Value;
            var updateDto = Fixture.Build<UpdateAddressDto>()
                .With(x => x.Street, "Updated Street")
                .With(x => x.Apartment, "Updated Apt")
                .With(x => x.City, "Updated City")
                .With(x => x.Zip, "54321")
                .With(x => x.Country, "Updated Country")
                .Create();

            // Act
            var result = address.Update(updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            address.Street.Should().Be(updateDto.Street);
            address.Apartment.Should().Be(updateDto.Apartment);
            address.City.Should().Be(updateDto.City);
            address.Zip.Should().Be(updateDto.Zip);
            address.Country.Should().Be(updateDto.Country);
            address.Region.Should().Be(updateDto.Region);
        }

        [Fact]
        public void Fail_WithNullDto()
        {
            // Arrange
            var address = AddressEntity.Create(Create<CreateAddressDto>()).Value;

            // Act
            var result = address.Update(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Fail_WithInvalidProperties(string invalidValue)
        {
            // Arrange
            var address = AddressEntity.Create(Create<CreateAddressDto>()).Value;
            var updateDto = Fixture.Build<UpdateAddressDto>()
                .With(x => x.Street, invalidValue)
                .Create();

            // Act
            var result = address.Update(updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
        }
    }
}
