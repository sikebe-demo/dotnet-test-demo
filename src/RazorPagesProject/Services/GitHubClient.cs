using System.Net;
using System.Text.Json.Serialization;

namespace RazorPagesProject.Services;

public class GitHubClient(HttpClient client) : IGitHubClient
{
    public HttpClient Client { get; } = client;

    public async Task<GitHubUser?> GetUserAsync(string userName)
    {
        try
        {
            var response = await Client.GetAsync($"/users/{Uri.EscapeDataString(userName)}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
            return user;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            // Re-throw other HTTP errors (rate limiting, server errors, etc.)
            throw;
        }
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
    public string? Name { get; set; }

    [JsonPropertyName("company")]
    public string? Company { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("bio")]
    public string? Bio { get; set; }

    [JsonPropertyName("followers")]
    public int Followers { get; set; }

    [JsonPropertyName("following")]
    public int Following { get; set; }

    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
