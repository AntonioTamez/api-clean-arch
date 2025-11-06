using CleanArch.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArch.Domain.Tests.ValueObjects;

public class ApplicationVersionTests
{
    [Theory]
    [InlineData("1.0.0")]
    [InlineData("2.1.0")]
    [InlineData("10.20.30")]
    [InlineData("0.0.1")]
    public void Create_WithValidSemVer_ShouldSucceed(string version)
    {
        // Act
        var result = ApplicationVersion.Create(version);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ToString().Should().Be(version);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmpty_ShouldFail(string invalidVersion)
    {
        // Act
        var result = ApplicationVersion.Create(invalidVersion);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("1.0")]           // Falta patch
    [InlineData("1")]             // Solo major
    [InlineData("v1.0.0")]        // Prefijo no válido
    [InlineData("1.0.0.0")]       // Demasiados componentes
    [InlineData("1.a.0")]         // Componente no numérico
    [InlineData("-1.0.0")]        // Negativo
    public void Create_WithInvalidFormat_ShouldFail(string invalidVersion)
    {
        // Act
        var result = ApplicationVersion.Create(invalidVersion);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithMajorMinorPatch_ShouldSetProperties()
    {
        // Act
        var result = ApplicationVersion.Create("2.5.7");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Major.Should().Be(2);
        result.Value.Minor.Should().Be(5);
        result.Value.Patch.Should().Be(7);
    }

    [Fact]
    public void CompareTo_GreaterVersion_ShouldReturnPositive()
    {
        // Arrange
        var version1 = ApplicationVersion.Create("2.0.0").Value;
        var version2 = ApplicationVersion.Create("1.0.0").Value;

        // Act
        var comparison = version1.CompareTo(version2);

        // Assert
        comparison.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CompareTo_LesserVersion_ShouldReturnNegative()
    {
        // Arrange
        var version1 = ApplicationVersion.Create("1.0.0").Value;
        var version2 = ApplicationVersion.Create("2.0.0").Value;

        // Act
        var comparison = version1.CompareTo(version2);

        // Assert
        comparison.Should().BeLessThan(0);
    }

    [Fact]
    public void CompareTo_SameVersion_ShouldReturnZero()
    {
        // Arrange
        var version1 = ApplicationVersion.Create("1.5.3").Value;
        var version2 = ApplicationVersion.Create("1.5.3").Value;

        // Act
        var comparison = version1.CompareTo(version2);

        // Assert
        comparison.Should().Be(0);
    }

    [Fact]
    public void GreaterThan_Operator_ShouldWorkCorrectly()
    {
        // Arrange
        var version1 = ApplicationVersion.Create("2.0.0").Value;
        var version2 = ApplicationVersion.Create("1.0.0").Value;

        // Act & Assert
        (version1 > version2).Should().BeTrue();
        (version2 > version1).Should().BeFalse();
    }

    [Fact]
    public void IncrementMajor_ShouldResetMinorAndPatch()
    {
        // Arrange
        var version = ApplicationVersion.Create("1.5.3").Value;

        // Act
        var newVersion = version.IncrementMajor();

        // Assert
        newVersion.Major.Should().Be(2);
        newVersion.Minor.Should().Be(0);
        newVersion.Patch.Should().Be(0);
    }

    [Fact]
    public void IncrementMinor_ShouldResetPatch()
    {
        // Arrange
        var version = ApplicationVersion.Create("1.5.3").Value;

        // Act
        var newVersion = version.IncrementMinor();

        // Assert
        newVersion.Major.Should().Be(1);
        newVersion.Minor.Should().Be(6);
        newVersion.Patch.Should().Be(0);
    }

    [Fact]
    public void IncrementPatch_ShouldOnlyIncrementPatch()
    {
        // Arrange
        var version = ApplicationVersion.Create("1.5.3").Value;

        // Act
        var newVersion = version.IncrementPatch();

        // Assert
        newVersion.Major.Should().Be(1);
        newVersion.Minor.Should().Be(5);
        newVersion.Patch.Should().Be(4);
    }
}
