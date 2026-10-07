namespace ApiCleanArch.Infrastructure.Users;

internal static class InMemoryUserSeed
{
    private static readonly DateTimeOffset SeededAt = new(2025, 1, 15, 9, 0, 0, TimeSpan.Zero);

    public static IReadOnlyList<UserRecord> Records { get; } =
    [
        new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Ada Lovelace", "ada.lovelace@example.com", SeededAt, null),
        new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Grace Hopper", "grace.hopper@example.com", SeededAt.AddDays(1), null),
        new(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Alan Turing", "alan.turing@example.com", SeededAt.AddDays(2), SeededAt.AddDays(10)),
        new(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Margaret Hamilton", "margaret.hamilton@example.com", SeededAt.AddDays(3), null),
        new(Guid.Parse("55555555-5555-5555-5555-555555555555"), "Linus Torvalds", "linus.torvalds@example.com", SeededAt.AddDays(4), null),
    ];
}
