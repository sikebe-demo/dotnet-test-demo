using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesProject.Services;
using Xunit;

namespace RazorPagesProject.IntegrationTests;

public class GitHubClientTests
{
    [Fact]
    public async Task GetUserAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler());
        httpClient.BaseAddress = new Uri("https://api.github.com");
        var gitHubClient = new GitHubClient(httpClient);

        // Act
        var result = await gitHubClient.GetUserAsync("nonexistentuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler());
        httpClient.BaseAddress = new Uri("https://api.github.com");
        var gitHubClient = new GitHubClient(httpClient);

        // Act
        var result = await gitHubClient.GetUserAsync("octocat");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("octocat", result.Login);
        Assert.Equal("The Octocat", result.Name);
        Assert.Equal("GitHub", result.Company);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri!.ToString();
            
            if (uri.Contains("/users/nonexistentuser"))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            
            if (uri.Contains("/users/octocat"))
            {
                var user = new GitHubUser
                {
                    Login = "octocat",
                    Name = "The Octocat",
                    Company = "GitHub"
                };
                var json = JsonSerializer.Serialize(user);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                });
            }
            
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }
    }
}