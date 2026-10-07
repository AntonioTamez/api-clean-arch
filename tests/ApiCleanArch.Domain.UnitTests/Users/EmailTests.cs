using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Domain.UnitTests.Users;

public sealed class EmailTests
{
    [Fact]
    public void Create_TrimsAndLowercasesValue()
    {
        var email = Email.Create("  John.Doe@Example.COM ");

        Assert.Equal("john.doe@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-at-sign")]
    [InlineData("@example.com")]
    [InlineData("john@")]
    [InlineData("john@@example.com")]
    [InlineData("john doe@example.com")]
    [InlineData("john@example")]
    public void Create_RejectsInvalidFormat(string raw)
    {
        Assert.Throws<InvalidEmailException>(() => Email.Create(raw));
    }

    [Fact]
    public void Create_RejectsValueLongerThanMaximum()
    {
        var raw = new string('a', Email.MaxLength) + "@example.com";

        Assert.Throws<InvalidEmailException>(() => Email.Create(raw));
    }

    [Fact]
    public void Equality_IgnoresCasingOfOriginalInput()
    {
        Assert.Equal(Email.Create("A@b.com"), Email.Create("a@B.com"));
    }
}
