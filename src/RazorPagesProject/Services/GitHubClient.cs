using System.Text.Json.Serialization;
using System.Net;

namespace RazorPagesProject.Services;

public class GitHubClient(HttpClient client) : IGitHubClient
{
    public HttpClient Client { get; } = client;

    public async Task<GitHubUserResult> GetUserAsync(string userName)
    {
        var response = await Client.GetAsync($"/users/{Uri.EscapeDataString(userName)}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return GitHubUserResult.UserNotFound(userName);
        }
        
        if (!response.IsSuccessStatusCode)
        {
            return GitHubUserResult.Error($"GitHub API error: {response.StatusCode}");
        }

        var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
        return GitHubUserResult.Success(user!);
    }
}

public interface IGitHubClient
{
    Task<GitHubUserResult> GetUserAsync(string userName);
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

public class GitHubUserResult
{
    public bool IsSuccess { get; private set; }
    public bool IsUserNotFound { get; private set; }
    public GitHubUser? User { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? RequestedUserName { get; private set; }

    private GitHubUserResult() { }

    public static GitHubUserResult Success(GitHubUser user)
    {
        return new GitHubUserResult
        {
            IsSuccess = true,
            User = user
        };
    }

    public static GitHubUserResult UserNotFound(string userName)
    {
        return new GitHubUserResult
        {
            IsUserNotFound = true,
            RequestedUserName = userName,
            ErrorMessage = $"GitHub user '{userName}' not found"
        };
    }

    public static GitHubUserResult Error(string message)
    {
        return new GitHubUserResult
        {
            ErrorMessage = message
        };
    }
}
