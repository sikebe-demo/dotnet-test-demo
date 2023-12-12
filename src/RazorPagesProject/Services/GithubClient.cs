namespace RazorPagesProject.Services;

public class GithubClient(HttpClient client) : IGithubClient
{
    public HttpClient Client { get; } = client;

    public async Task<GithubUser> GetUserAsync(string userName)
    {
        var response = await Client.GetAsync($"/users/{Uri.EscapeDataString(userName)}");
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<GithubUser>();
        return user!;
    }
}

public interface IGithubClient
{
    Task<GithubUser> GetUserAsync(string userName);
}

public class GithubUser
{
    public required string Login { get; set; }
    public required string Name { get; set; }
    public required string Company { get; set; }
}
