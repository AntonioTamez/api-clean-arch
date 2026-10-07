using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiCleanArch.Api.IntegrationTests.Users;

public sealed class UsersApiTests : IDisposable
{
    private const string BaseUrl = "/api/v1/users";
    private const string SeededAdaId = "11111111-1111-1111-1111-111111111111";
    private const string SeededGraceEmail = "grace.hopper@example.com";

    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public UsersApiTests() => _client = _factory.CreateClient();

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed record UserResponse(Guid Id, string Name, string Email, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

    private static object Body(string name, string email) => new { name, email };

    [Fact]
    public async Task List_ReturnsSeededUsers()
    {
        var users = await _client.GetFromJsonAsync<List<UserResponse>>(BaseUrl);

        Assert.NotNull(users);
        Assert.Equal(5, users.Count);
        Assert.Contains(users, u => u.Id == Guid.Parse(SeededAdaId) && u.Email == "ada.lovelace@example.com");
    }

    [Fact]
    public async Task Get_ReturnsSeededUserById()
    {
        var user = await _client.GetFromJsonAsync<UserResponse>($"{BaseUrl}/{SeededAdaId}");

        Assert.NotNull(user);
        Assert.Equal("Ada Lovelace", user.Name);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundProblemForUnknownId()
    {
        var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationAndPersistsUser()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, Body("Katherine Johnson", "Katherine.Johnson@Example.com"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(created);
        Assert.Equal("katherine.johnson@example.com", created.Email);
        Assert.EndsWith($"{BaseUrl}/{created.Id}", response.Headers.Location?.ToString());

        var fetched = await _client.GetFromJsonAsync<UserResponse>(response.Headers.Location);
        Assert.Equal(created, fetched);
    }

    [Fact]
    public async Task Create_ReturnsConflictWhenEmailAlreadyInUse()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, Body("Another Grace", SeededGraceEmail));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("", "valid@example.com")]
    [InlineData("Valid Name", "not-an-email")]
    [InlineData("Valid Name", "")]
    public async Task Create_ReturnsBadRequestProblemForInvalidBody(string name, string email)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, Body(name, email));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Create_ReturnsBadRequestWhenDomainRuleRejectsTrimmedName()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, Body(" a ", "short.name@example.com"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(400, problem.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Update_ReturnsUpdatedUser()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{SeededAdaId}", Body("Ada King", "ada.king@example.com"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(updated);
        Assert.Equal("Ada King", updated.Name);
        Assert.Equal("ada.king@example.com", updated.Email);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task Update_ReturnsNotFoundForUnknownId()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", Body("Ada King", "ada.king@example.com"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsConflictWhenEmailBelongsToAnotherUser()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{SeededAdaId}", Body("Ada Lovelace", SeededGraceEmail));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsBadRequestForInvalidBody()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{SeededAdaId}", Body("Ada Lovelace", "bad"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentThenNotFound()
    {
        var first = await _client.DeleteAsync($"{BaseUrl}/{SeededAdaId}");
        var get = await _client.GetAsync($"{BaseUrl}/{SeededAdaId}");
        var second = await _client.DeleteAsync($"{BaseUrl}/{SeededAdaId}");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundForRouteThatIsNotAGuid()
    {
        var response = await _client.GetAsync($"{BaseUrl}/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
