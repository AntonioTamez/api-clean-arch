namespace ApiCleanArch.Domain.Users;

public readonly record struct UserId
{
    private UserId(Guid value) => Value = value;

    public Guid Value { get; }

    public static UserId New() => new(Guid.NewGuid());

    public static UserId Create(Guid value) =>
        value == Guid.Empty ? throw new InvalidUserIdException() : new UserId(value);

    public override string ToString() => Value.ToString();
}
