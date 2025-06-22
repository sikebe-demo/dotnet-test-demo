using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class GitHubProfileTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public GitHubProfileTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetGitHubProfile_WithNonExistentUser_ShouldReturnPageWithoutError()
    {
        // Arrange
        var nonExistentUsername = "あああああああああああああああああああああ";

        // Act
        var response = await _client.GetAsync($"/GitHubProfile/{nonExistentUsername}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("GitHub", content); // Should show the page
        Assert.DoesNotContain("HttpRequestException", content); // Should not show exception
        Assert.DoesNotContain("An error occurred", content); // Should not show error page
    }

    [Fact]
    public async Task GetGitHubProfile_WithValidUser_ShouldReturnPageWithUserInfo()
    {
        // Arrange - using a known GitHub user
        var validUsername = "msftgits";

        // Act
        var response = await _client.GetAsync($"/GitHubProfile/{validUsername}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("GitHub", content);
        Assert.Contains(validUsername, content);
    }
}