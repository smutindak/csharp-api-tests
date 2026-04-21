using NUnit.Framework;
using Shouldly;
using CSharpApiTests.Services;
using CSharpApiTests.Models;

namespace CSharpApiTests.Tests;

[TestFixture]
public class DbTests
{
    private DbService _db = null!;

    [SetUp]
    public void Setup()
    {
        _db = new DbService();
    }

    [Test]
    public async Task GetAllUsers_ReturnsSeededRows()
    {
        var users = await _db.GetAllUsersAsync();

        users.ShouldNotBeNull();
        users.Count().ShouldBeGreaterThanOrEqualTo(3);
    }

    [Test]
    public async Task GetUserByEmail_ValidEmail_ReturnsCorrectUser()
    {
        var user = await _db.GetUserByEmailAsync("alice@example.com");

        user.ShouldNotBeNull();
        user.Name.ShouldBe("Alice Kamau");
        user.Role.ShouldBe("admin");
        user.IsActive.ShouldBeTrue();
    }

    [Test]
    public async Task GetUserByEmail_UnknownEmail_ReturnsNull()
    {
        var user = await _db.GetUserByEmailAsync("nobody@example.com");

        user.ShouldBeNull();
    }

    [Test]
    public async Task InsertUser_ThenQueryBack_VerifiesCorrectData()
    {
        var newUser = new User
        {
            Name = "Test User",
            Email = $"test_{Guid.NewGuid()}@example.com",
            Role = "user"
        };

        var insertedId = await _db.InsertUserAsync(newUser);

        insertedId.ShouldBeGreaterThan(0);

        var fetched = await _db.GetUserByIdAsync(insertedId);

        fetched.ShouldNotBeNull();
        fetched.Name.ShouldBe(newUser.Name);
        fetched.Email.ShouldBe(newUser.Email);
        fetched.IsActive.ShouldBeTrue();

        await _db.DeleteUserAsync(insertedId);
    }

    [Test]
    public async Task DeleteUser_ThenQuery_ReturnsNull()
    {
        var tempUser = new User
        {
            Name = "To Be Deleted",
            Email = $"delete_{Guid.NewGuid()}@example.com",
            Role = "user"
        };

        var id = await _db.InsertUserAsync(tempUser);
        await _db.DeleteUserAsync(id);

        var result = await _db.GetUserByIdAsync(id);
        result.ShouldBeNull();
    }
}
