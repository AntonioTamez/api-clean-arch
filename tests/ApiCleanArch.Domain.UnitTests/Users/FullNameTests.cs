using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Domain.UnitTests.Users;

public sealed class FullNameTests
{
    [Fact]
    public void Create_TrimsValue()
    {
        Assert.Equal("Ada Lovelace", FullName.Create("  Ada Lovelace  ").Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A")]
    [InlineData(" A ")]
    public void Create_RejectsMissingOrTooShortValue(string raw)
    {
        Assert.Throws<InvalidFullNameException>(() => FullName.Create(raw));
    }

    [Fact]
    public void Create_RejectsValueLongerThanMaximum()
    {
        Assert.Throws<InvalidFullNameException>(() => FullName.Create(new string('x', FullName.MaxLength + 1)));
    }

    [Fact]
    public void Create_AcceptsBoundaryLengths()
    {
        Assert.Equal(FullName.MinLength, FullName.Create(new string('x', FullName.MinLength)).Value.Length);
        Assert.Equal(FullName.MaxLength, FullName.Create(new string('x', FullName.MaxLength)).Value.Length);
    }
}
