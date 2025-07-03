using System.Net;
using System.Text.Json.Serialization;

namespace RazorPagesProject.Services;

public class GitHubClient(HttpClient client) : IGitHubClient
{
    public HttpClient Client { get; } = client;

    public async Task<GitHubUser?> GetUserAsync(string userName)
    {
        var response = await Client.GetAsync($"/users/{Uri.EscapeDataString(userName)}");
        
        // Handle user not found (404) gracefully
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        // For other errors, still throw exception as they are unexpected
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
        return user!;
    }
}

public interface IGitHubClient
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
