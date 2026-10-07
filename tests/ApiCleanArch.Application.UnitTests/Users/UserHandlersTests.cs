using ApiCleanArch.Application.Users;
using ApiCleanArch.Application.Users.Create;
using ApiCleanArch.Application.Users.Delete;
using ApiCleanArch.Application.Users.Get;
using ApiCleanArch.Application.Users.List;
using ApiCleanArch.Application.Users.Update;
using ApiCleanArch.Domain.Users;
using Microsoft.Extensions.Time.Testing;

namespace ApiCleanArch.Application.UnitTests.Users;

public sealed class UserHandlersTests
{
    private static readonly DateTimeOffset Start = new(2025, 3, 1, 8, 0, 0, TimeSpan.Zero);

    private readonly FakeUserRepository _repository = new();
    private readonly FakeTimeProvider _time = new(Start);
    private readonly CancellationToken _ct = CancellationToken.None;

    private async Task<User> SeedAsync(string name, string email)
    {
        var user = User.Create(FullName.Create(name), Email.Create(email), Start);
        await _repository.AddAsync(user, _ct);
        return user;
    }

    [Fact]
    public async Task ListUsers_ReturnsAllUsersAsDtos()
    {
        await SeedAsync("Ada Lovelace", "ada@example.com");
        await SeedAsync("Grace Hopper", "grace@example.com");

        var result = await new ListUsersHandler(_repository).Handle(new ListUsersQuery(), _ct);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Email == "ada@example.com");
    }

    [Fact]
    public async Task GetUser_ReturnsDtoForExistingUser()
    {
        var user = await SeedAsync("Ada Lovelace", "ada@example.com");

        var dto = await new GetUserHandler(_repository).Handle(new GetUserQuery(user.Id.Value), _ct);

        Assert.Equal(new UserDto(user.Id.Value, "Ada Lovelace", "ada@example.com", Start, null), dto);
    }

    [Fact]
    public async Task GetUser_ThrowsNotFoundForUnknownId()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            new GetUserHandler(_repository).Handle(new GetUserQuery(Guid.NewGuid()), _ct));
    }

    [Fact]
    public async Task GetUser_ThrowsDomainErrorForEmptyId()
    {
        await Assert.ThrowsAsync<InvalidUserIdException>(() =>
            new GetUserHandler(_repository).Handle(new GetUserQuery(Guid.Empty), _ct));
    }

    [Fact]
    public async Task CreateUser_PersistsNormalizedUserAndReturnsDto()
    {
        var handler = new CreateUserHandler(_repository, _time);

        var dto = await handler.Handle(new CreateUserCommand("  Ada Lovelace ", "ADA@Example.com"), _ct);

        Assert.Equal("Ada Lovelace", dto.Name);
        Assert.Equal("ada@example.com", dto.Email);
        Assert.Equal(Start, dto.CreatedAt);
        Assert.Null(dto.UpdatedAt);
        Assert.Equal(1, _repository.Count);
    }

    [Fact]
    public async Task CreateUser_ThrowsConflictWhenEmailAlreadyInUse()
    {
        await SeedAsync("Ada Lovelace", "ada@example.com");

        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() =>
            new CreateUserHandler(_repository, _time).Handle(new CreateUserCommand("Other Person", "Ada@example.com"), _ct));
        Assert.Equal(1, _repository.Count);
    }

    [Fact]
    public async Task CreateUser_ThrowsDomainErrorForInvalidInput()
    {
        await Assert.ThrowsAsync<InvalidEmailException>(() =>
            new CreateUserHandler(_repository, _time).Handle(new CreateUserCommand("Ada Lovelace", "nope"), _ct));
        await Assert.ThrowsAsync<InvalidFullNameException>(() =>
            new CreateUserHandler(_repository, _time).Handle(new CreateUserCommand("A", "a@example.com"), _ct));
    }

    [Fact]
    public async Task UpdateUser_ChangesNameAndEmailAndStampsUpdate()
    {
        var user = await SeedAsync("Ada Lovelace", "ada@example.com");
        _time.Advance(TimeSpan.FromHours(2));

        var dto = await new UpdateUserHandler(_repository, _time)
            .Handle(new UpdateUserCommand(user.Id.Value, "Grace Hopper", "grace@example.com"), _ct);

        Assert.Equal("Grace Hopper", dto.Name);
        Assert.Equal("grace@example.com", dto.Email);
        Assert.Equal(Start.AddHours(2), dto.UpdatedAt);
    }

    [Fact]
    public async Task UpdateUser_AllowsKeepingOwnEmail()
    {
        var user = await SeedAsync("Ada Lovelace", "ada@example.com");

        var dto = await new UpdateUserHandler(_repository, _time)
            .Handle(new UpdateUserCommand(user.Id.Value, "Ada King", "ada@example.com"), _ct);

        Assert.Equal("Ada King", dto.Name);
    }

    [Fact]
    public async Task UpdateUser_ThrowsNotFoundForUnknownId()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            new UpdateUserHandler(_repository, _time)
                .Handle(new UpdateUserCommand(Guid.NewGuid(), "Grace Hopper", "g@example.com"), _ct));
    }

    [Fact]
    public async Task UpdateUser_ThrowsConflictWhenEmailBelongsToAnotherUser()
    {
        var ada = await SeedAsync("Ada Lovelace", "ada@example.com");
        await SeedAsync("Grace Hopper", "grace@example.com");

        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() =>
            new UpdateUserHandler(_repository, _time)
                .Handle(new UpdateUserCommand(ada.Id.Value, "Ada Lovelace", "grace@example.com"), _ct));
    }

    [Fact]
    public async Task DeleteUser_RemovesExistingUser()
    {
        var user = await SeedAsync("Ada Lovelace", "ada@example.com");

        await new DeleteUserHandler(_repository).Handle(new DeleteUserCommand(user.Id.Value), _ct);

        Assert.Equal(0, _repository.Count);
    }

    [Fact]
    public async Task DeleteUser_ThrowsNotFoundForUnknownId()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            new DeleteUserHandler(_repository).Handle(new DeleteUserCommand(Guid.NewGuid()), _ct));
    }
}
