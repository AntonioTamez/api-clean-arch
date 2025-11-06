using CleanArch.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArch.Domain.Tests.ValueObjects;

public class ProjectCodeTests
{
    [Fact]
    public void Create_WithValidCode_ShouldSucceed()
    {
        // Arrange
        var code = "PRJ-2024-001";

        // Act
        var result = ProjectCode.Create(code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmptyCode_ShouldFail(string invalidCode)
    {
        // Act
        var result = ProjectCode.Create(invalidCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot be empty");
    }

    [Theory]
    [InlineData("AB")]           // Muy corto
    [InlineData("A")]            // Muy corto
    public void Create_WithInvalidLength_TooShort_ShouldFail(string invalidCode)
    {
        // Act
        var result = ProjectCode.Create(invalidCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("at least 3 characters");
    }

    [Fact]
    public void Create_WithInvalidLength_TooLong_ShouldFail()
    {
        // Arrange
        var invalidCode = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456"; // >30 caracteres

        // Act
        var result = ProjectCode.Create(invalidCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot exceed 30 characters");
    }

    [Theory]
    [InlineData("PRJ-2024-001")]
    [InlineData("BACKEND-API")]
    [InlineData("MOBILE_APP")]
    [InlineData("WEB.PORTAL")]
    public void Create_WithValidFormats_ShouldSucceed(string validCode)
    {
        // Act
        var result = ProjectCode.Create(validCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("prj-2024-001")]  // Lowercase
    [InlineData("Prj-2024-001")]  // Mixed case
    public void Create_WithLowercase_ShouldConvertToUppercase(string lowercaseCode)
    {
        // Act
        var result = ProjectCode.Create(lowercaseCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(lowercaseCode.ToUpper());
    }

    [Fact]
    public void Equals_WithSameCode_ShouldBeEqual()
    {
        // Arrange
        var code1 = ProjectCode.Create("PRJ-2024-001").Value;
        var code2 = ProjectCode.Create("PRJ-2024-001").Value;

        // Act & Assert
        code1.Should().Be(code2);
        (code1 == code2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCode_ShouldNotBeEqual()
    {
        // Arrange
        var code1 = ProjectCode.Create("PRJ-2024-001").Value;
        var code2 = ProjectCode.Create("PRJ-2024-002").Value;

        // Act & Assert
        code1.Should().NotBe(code2);
        (code1 != code2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameCode_ShouldBeEqual()
    {
        // Arrange
        var code1 = ProjectCode.Create("PRJ-2024-001").Value;
        var code2 = ProjectCode.Create("PRJ-2024-001").Value;

        // Act & Assert
        code1.GetHashCode().Should().Be(code2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnCode()
    {
        // Arrange
        var code = ProjectCode.Create("PRJ-2024-001").Value;

        // Act
        var result = code.ToString();

        // Assert
        result.Should().Be("PRJ-2024-001");
    }
}
