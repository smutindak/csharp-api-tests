using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Shouldly;
using System.Text.Json;

namespace CSharpApiTests.Tests;

[TestFixture]
public class PlaywrightApiTests : PlaywrightTest
{
    private IAPIRequestContext _request = null!;

    [SetUp]
    public async Task SetUpApiContext()
    {
        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://jsonplaceholder.typicode.com",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            }
        });
    }

    [TearDown]
    public async Task TearDownApiContext()
    {
        await _request.DisposeAsync();
    }

    [Test]
    public async Task GetPost_ValidId_Returns200()
    {
        var response = await _request.GetAsync("/posts/1");

        response.Status.ShouldBe(200);
        response.Ok.ShouldBeTrue();

        var body = await response.JsonAsync();
        body.HasValue.ShouldBeTrue();

        var id = body.Value.GetProperty("id").GetInt32();
        var title = body.Value.GetProperty("title").GetString();

        id.ShouldBe(1);
        title.ShouldNotBeNullOrEmpty();
    }

    [Test]
    public async Task CreatePost_ValidPayload_Returns201()
    {
        var response = await _request.PostAsync("/posts", new APIRequestContextOptions
        {
            DataObject = new
            {
                userId = 1,
                title = "Playwright C# API Test",
                body = "Testing with IAPIRequestContext"
            }
        });

        response.Status.ShouldBe(201);

        var body = await response.JsonAsync();
        body.HasValue.ShouldBeTrue();

        var id = body.Value.GetProperty("id").GetInt32();
        var title = body.Value.GetProperty("title").GetString();

        id.ShouldBeGreaterThan(0);
        title.ShouldBe("Playwright C# API Test");
    }

    [Test]
    public async Task GetPosts_ReturnsCollection()
    {
        var response = await _request.GetAsync("/posts");

        response.Status.ShouldBe(200);

        var body = await response.JsonAsync();
        body.HasValue.ShouldBeTrue();

        var posts = body.Value.EnumerateArray().ToList();
        posts.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task DeletePost_ValidId_Returns200()
    {
        var response = await _request.DeleteAsync("/posts/1");

        response.Status.ShouldBe(200);
        response.Ok.ShouldBeTrue();
    }
}
