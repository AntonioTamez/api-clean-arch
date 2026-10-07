namespace ApiCleanArch.Domain.Users;

public sealed class User
{
    private User(UserId id, FullName name, Email email, DateTimeOffset createdAt, DateTimeOffset? updatedAt)
    {
        Id = id;
        Name = name;
        Email = email;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public UserId Id { get; }

    public FullName Name { get; private set; }

    public Email Email { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static User Create(FullName name, Email email, DateTimeOffset now) =>
        new(UserId.New(), name, email, now, null);

    public static User Rehydrate(
        UserId id,
        FullName name,
        Email email,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt) =>
        new(id, name, email, createdAt, updatedAt);

    public void Rename(FullName name, DateTimeOffset now)
    {
        EnsureNotBeforeCreation(now);
        Name = name;
        UpdatedAt = now;
    }

    public void ChangeEmail(Email email, DateTimeOffset now)
    {
        EnsureNotBeforeCreation(now);
        Email = email;
        UpdatedAt = now;
    }

    private void EnsureNotBeforeCreation(DateTimeOffset now)
    {
        if (now < CreatedAt)
        {
            throw new InvalidUserChangeException("A user cannot be changed before it was created.");
        }
    }
}
