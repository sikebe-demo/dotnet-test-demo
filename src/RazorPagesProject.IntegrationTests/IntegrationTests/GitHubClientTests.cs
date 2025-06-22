using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using RazorPagesProject.Services;
using System.Net;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class GitHubClientTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GitHubClientTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUserAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var client = _factory.CreateClient();
        var services = _factory.Services;
        var gitHubClient = (IGitHubClient)services.GetService(typeof(IGitHubClient))!;

        // Act
        var result = await gitHubClient.GetUserAsync("nonexistentuser123");
        
        // Assert - Should return null instead of throwing exception
        Assert.Null(result);
    }

    [Fact]
    public async Task GitHubProfile_WithNonExistentUser_ShouldReturnOKWithNoUserProfile()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Try to access profile page for non-existent user
        var response = await client.GetAsync("/GitHubProfile/nonexistentuser123");

        // Assert - Should return 200 OK and display the search form without user profile
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        // Should contain the search form but not the user profile section
        Assert.Contains("fas fa-search", content); // Search form should be present
        Assert.DoesNotContain("id=\"user-login\"", content); // User profile should not be present
    }

    [Fact]
    public async Task GetUserAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        var services = _factory.Services;
        var gitHubClient = (IGitHubClient)services.GetService(typeof(IGitHubClient))!;

        // Act
        var result = await gitHubClient.GetUserAsync("octocat");
        
        // Assert - Should return a user object (real API call)
        Assert.NotNull(result);
        Assert.Equal("octocat", result.Login);
    }
}