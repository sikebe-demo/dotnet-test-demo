using System.Net;
using System.Text;
using System.Text.Json;
using RazorPagesProject.Services;
using Xunit;

namespace RazorPagesProject.IntegrationTests.Services;

public class GitHubClientTests
{
    [Fact]
    public async Task GetUserAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var testUser = new GitHubUser
        {
            Login = "octocat",
            Name = "The Octocat",
            Company = "GitHub"
        };
        
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, testUser);
        var githubClient = new GitHubClient(httpClient);
        
        // Act
        var result = await githubClient.GetUserAsync("octocat");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("octocat", result.Login);
        Assert.Equal("The Octocat", result.Name);
        Assert.Equal("GitHub", result.Company);
    }
    
    [Fact]
    public async Task GetUserAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.NotFound, null);
        var githubClient = new GitHubClient(httpClient);
        
        // Act
        var result = await githubClient.GetUserAsync("nonexistentuser");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUserAsync_WhenServerError_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, null);
        var githubClient = new GitHubClient(httpClient);
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            githubClient.GetUserAsync("testuser"));
    }
    
    private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, GitHubUser? user)
    {
        var handler = new MockHttpMessageHandler(statusCode, user);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };
    }
}

internal class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly GitHubUser? _user;
    
    public MockHttpMessageHandler(HttpStatusCode statusCode, GitHubUser? user)
    {
        _statusCode = statusCode;
        _user = user;
    }
    
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode);
        
        if (_statusCode == HttpStatusCode.OK && _user != null)
        {
            var json = JsonSerializer.Serialize(_user, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        
        return Task.FromResult(response);
    }
}