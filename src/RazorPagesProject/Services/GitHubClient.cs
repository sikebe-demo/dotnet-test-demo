using System.Net;
using System.Text.Json.Serialization;

namespace RazorPagesProject.Services;

public class GitHubClient(HttpClient client, ILogger<GitHubClient> logger) : IGitHubClient
{
    public HttpClient Client { get; } = client;

    public async Task<GitHubUserResult> GetUserAsync(string userName)
    {
        try
        {
            logger.LogInformation("Fetching GitHub user profile for: {UserName}", userName);
            
            var response = await Client.GetAsync($"/users/{Uri.EscapeDataString(userName)}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("GitHub user not found: {UserName}", userName);
                return GitHubUserResult.NotFound();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("GitHub API error: {StatusCode} {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return GitHubUserResult.Error($"GitHub API error: {response.StatusCode}");
            }

            var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
            if (user == null)
            {
                logger.LogError("Failed to deserialize GitHub user response for: {UserName}", userName);
                return GitHubUserResult.Error("Failed to parse user data");
            }

            logger.LogInformation("Successfully fetched GitHub user profile for: {UserName}", userName);
            return GitHubUserResult.Success(user);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed when fetching user: {UserName}", userName);
            return GitHubUserResult.Error("Network error occurred while fetching user data");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            logger.LogError(ex, "Request timeout when fetching user: {UserName}", userName);
            return GitHubUserResult.Error("Request timed out");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error when fetching user: {UserName}", userName);
            return GitHubUserResult.Error("An unexpected error occurred");
        }
    }
}

public interface IGitHubClient
{
    Task<GitHubUserResult> GetUserAsync(string userName);
}

public class GitHubUserResult
{
    public bool IsSuccess { get; init; }
    public GitHubUser? User { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsNotFound { get; init; }

    public static GitHubUserResult Success(GitHubUser user) => new() { IsSuccess = true, User = user };
    public static GitHubUserResult NotFound() => new() { IsNotFound = true };
    public static GitHubUserResult Error(string message) => new() { ErrorMessage = message };
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

    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; set; }

    [JsonPropertyName("followers")]
    public int Followers { get; set; }

    [JsonPropertyName("following")]
    public int Following { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
}
