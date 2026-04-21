using NUnit.Framework;
using Shouldly;
using CSharpApiTests.Services;
using CSharpApiTests.Models;
using System.Net;

namespace CSharpApiTests.Tests;

[TestFixture]
public class ApiDbTests
{
    private PostsService _api = null!;
    private DbService _db = null!;
    private List<int> _insertedUserIds = new();

    [SetUp]
    public void Setup()
    {
        _api = new PostsService();
        _db = new DbService();
        _insertedUserIds = new List<int>();
    }

    [TearDown]
    public async Task Cleanup()
    {
        foreach (var id in _insertedUserIds)
            await _db.DeleteUserAsync(id);
    }

    [Test]
    public async Task CreateUser_InDb_ThenVerifyExists()
    {
        // Arrange
        var newUser = new User
        {
            Name = "Dave Mwangi",
            Email = $"dave_{Guid.NewGuid()}@example.com",
            Role = "user"
        };

        // Act — write to DB (simulating what an API POST would do)
        var id = await _db.InsertUserAsync(newUser);
        _insertedUserIds.Add(id);

        // Assert — verify DB state is correct
        var dbUser = await _db.GetUserByIdAsync(id);

        dbUser.ShouldNotBeNull();
        dbUser.Name.ShouldBe(newUser.Name);
        dbUser.Email.ShouldBe(newUser.Email);
        dbUser.Role.ShouldBe("user");
        dbUser.IsActive.ShouldBeTrue();
        dbUser.CreatedAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
    }

    [Test]
    public async Task ApiPost_ThenVerifyResponseMatchesExpectedShape()
    {
        // Arrange
        var newPost = new Post
        {
            UserId = 1,
            Title = "Backend validation test",
            Body = "Verifying API response shape"
        };

        // Act
        var response = await _api.CreatePostAsync(newPost);

        // Assert HTTP layer
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Data.ShouldNotBeNull();

        // Assert response shape — every field accounted for
        response.Data!.Id.ShouldBeGreaterThan(0);
        response.Data.Title.ShouldBe(newPost.Title);
        response.Data.Body.ShouldBe(newPost.Body);
        response.Data.UserId.ShouldBe(newPost.UserId);
    }

    [Test]
    public async Task CreateUser_ThenDeactivate_DbReflectsChange()
    {
        // Arrange — insert a user
        var user = new User
        {
            Name = "Eve Njeri",
            Email = $"eve_{Guid.NewGuid()}@example.com",
            Role = "user"
        };
        var id = await _db.InsertUserAsync(user);
        _insertedUserIds.Add(id);

        // Act — deactivate directly in DB (simulating an API PATCH /users/{id}/deactivate)
        using var conn = new Npgsql.NpgsqlConnection(
            "Host=localhost;Port=5432;Database=qadb;Username=qauser;Password=qapass");
        await conn.OpenAsync();
        await new Npgsql.NpgsqlCommand(
            $"UPDATE users SET is_active = false WHERE id = {id}", conn)
            .ExecuteNonQueryAsync();

        // Assert — DB reflects the deactivation
        var dbUser = await _db.GetUserByIdAsync(id);
        dbUser.ShouldNotBeNull();
        dbUser.IsActive.ShouldBeFalse();
    }

    [Test]
    public async Task GetAllUsers_CountMatchesDb()
    {
        // Act
        var users = await _db.GetAllUsersAsync();
        var count = users.Count();

        // Assert — at minimum the 3 seeded users exist
        count.ShouldBeGreaterThanOrEqualTo(3);

        // Assert specific seeded users are present
        users.ShouldContain(u => u.Email == "alice@example.com");
        users.ShouldContain(u => u.Email == "bob@example.com");
        users.ShouldContain(u => u.Email == "carol@example.com");
    }
}
