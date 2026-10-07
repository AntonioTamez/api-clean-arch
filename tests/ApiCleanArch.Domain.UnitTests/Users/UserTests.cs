using ApiCleanArch.Domain.Users;
using Microsoft.Extensions.Time.Testing;

namespace ApiCleanArch.Domain.UnitTests.Users;

public sealed class UserTests
{
    private static readonly DateTimeOffset Start = new(2025, 1, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly FakeTimeProvider _time = new(Start);

    private User NewUser() => User.Create(FullName.Create("Ada Lovelace"), Email.Create("ada@example.com"), _time.GetUtcNow());

    [Fact]
    public void Create_GeneratesIdAndSetsCreationTimestamp()
    {
        var user = NewUser();

        Assert.NotEqual(Guid.Empty, user.Id.Value);
        Assert.Equal(Start, user.CreatedAt);
        Assert.Null(user.UpdatedAt);
    }

    [Fact]
    public void Rename_ChangesNameAndSetsUpdatedAt()
    {
        var user = NewUser();
        _time.Advance(TimeSpan.FromHours(1));

        user.Rename(FullName.Create("Grace Hopper"), _time.GetUtcNow());

        Assert.Equal("Grace Hopper", user.Name.Value);
        Assert.Equal(Start.AddHours(1), user.UpdatedAt);
    }

    [Fact]
    public void ChangeEmail_ChangesEmailAndSetsUpdatedAt()
    {
        var user = NewUser();
        _time.Advance(TimeSpan.FromMinutes(5));

        user.ChangeEmail(Email.Create("grace@example.com"), _time.GetUtcNow());

        Assert.Equal("grace@example.com", user.Email.Value);
        Assert.Equal(Start.AddMinutes(5), user.UpdatedAt);
    }

    [Fact]
    public void Rename_RejectsChangeDatedBeforeCreation()
    {
        var user = NewUser();

        Assert.Throws<InvalidUserChangeException>(() => user.Rename(FullName.Create("Grace Hopper"), Start.AddSeconds(-1)));
    }

    [Fact]
    public void ChangeEmail_RejectsChangeDatedBeforeCreation()
    {
        var user = NewUser();

        Assert.Throws<InvalidUserChangeException>(() => user.ChangeEmail(Email.Create("x@example.com"), Start.AddSeconds(-1)));
    }

    [Fact]
    public void Rehydrate_RestoresStateWithoutAlteringIt()
    {
        var id = UserId.Create(Guid.NewGuid());
        var updated = Start.AddDays(1);

        var user = User.Rehydrate(id, FullName.Create("Ada Lovelace"), Email.Create("ada@example.com"), Start, updated);

        Assert.Equal(id, user.Id);
        Assert.Equal(Start, user.CreatedAt);
        Assert.Equal(updated, user.UpdatedAt);
    }
}
