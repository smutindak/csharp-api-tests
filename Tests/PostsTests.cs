using NUnit.Framework;
using Shouldly;
using CSharpApiTests.Services;
using CSharpApiTests.Models;
using System.Net;

namespace CSharpApiTests.Tests;

[TestFixture]
public class PostsTests
{
    private PostsService _postsService = null!;

    [SetUp]
    public void Setup()
    {
        _postsService = new PostsService();
    }

    [Test]
    public async Task GetPost_ValidId_Returns200WithCorrectData()
    {
        var post = await _postsService.GetPostAsync(1);

        post.ShouldNotBeNull();
        post.Id.ShouldBe(1);
        post.Title.ShouldNotBeNullOrEmpty();
        post.UserId.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task CreatePost_ValidPayload_Returns201WithCreatedData()
    {
        var newPost = new Post
        {
            UserId = 1,
            Title = "QA Automation Test Post",
            Body = "Created by C# NUnit test"
        };

        var response = await _postsService.CreatePostAsync(newPost);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Data.ShouldNotBeNull();
        response.Data!.Title.ShouldBe(newPost.Title);
        response.Data.Id.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task DeletePost_ValidId_Returns200()
    {
        var response = await _postsService.DeletePostAsync(1);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task GetPost_MultipleIds_AllReturnValidData(int postId)
    {
        var post = await _postsService.GetPostAsync(postId);

        post.ShouldNotBeNull();
        post.Id.ShouldBe(postId);
    }
}
