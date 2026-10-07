using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Domain.UnitTests.Users;

public sealed class UserIdTests
{
    [Fact]
    public void Create_RejectsEmptyGuid()
    {
        Assert.Throws<InvalidUserIdException>(() => UserId.Create(Guid.Empty));
    }

    [Fact]
    public void Create_KeepsGivenGuid()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid, UserId.Create(guid).Value);
    }

    [Fact]
    public void New_GeneratesNonEmptyUniqueIds()
    {
        var first = UserId.New();
        var second = UserId.New();

        Assert.NotEqual(Guid.Empty, first.Value);
        Assert.NotEqual(first, second);
    }
}
