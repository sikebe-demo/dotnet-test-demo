using System.Net;
using System.Text.Json.Serialization;

namespace RazorPagesProject.Services;

public class GithubClient(HttpClient client) : IGithubClient
{
    public HttpClient Client { get; } = client;

    public async Task<GitHubUser?> GetUserAsync(string userName)
    {
        var response = await Client.GetAsync($"/users/{Uri.EscapeDataString(userName)}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null; // ユーザーが存在しない場合はnullを返す
        }
        
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
        return user!;
    }
}

public interface IGithubClient
{
    Task<GitHubUser?> GetUserAsync(string userName);
}

public class GitHubUser
{

    [JsonPropertyName("login")]
    public required string Login { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("company")]
    public required string Company { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
}
