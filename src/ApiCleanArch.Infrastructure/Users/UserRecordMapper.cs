using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Infrastructure.Users;

internal static class UserRecordMapper
{
    public static UserRecord ToRecord(this User user) =>
        new(user.Id.Value, user.Name.Value, user.Email.Value, user.CreatedAt, user.UpdatedAt);

    public static User ToDomain(this UserRecord record) =>
        User.Rehydrate(
            UserId.Create(record.Id),
            FullName.Create(record.Name),
            Email.Create(record.Email),
            record.CreatedAt,
            record.UpdatedAt);
}
