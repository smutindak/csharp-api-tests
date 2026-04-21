using RestSharp;
using CSharpApiTests.Models;

namespace CSharpApiTests.Services;

public class PostsService
{
    private readonly RestClient _client;

    public PostsService()
    {
        _client = new RestClient("https://jsonplaceholder.typicode.com");
    }

    public async Task<Post?> GetPostAsync(int id)
    {
        var request = new RestRequest($"/posts/{id}");
        return await _client.GetAsync<Post>(request);
    }

    public async Task<RestResponse<Post>> CreatePostAsync(Post post)
    {
        var request = new RestRequest("/posts").AddJsonBody(post);
        return await _client.ExecutePostAsync<Post>(request);
    }

    public async Task<RestResponse> DeletePostAsync(int id)
    {
        var request = new RestRequest($"/posts/{id}");
        return await _client.DeleteAsync(request);
    }
}
