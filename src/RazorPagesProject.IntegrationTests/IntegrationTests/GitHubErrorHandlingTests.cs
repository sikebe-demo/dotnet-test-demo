using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesProject.IntegrationTests.Helpers;
using RazorPagesProject.Services;
using System.Net;
using Xunit;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class GitHubErrorHandlingTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GitHubErrorHandlingTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_GitHubProfilePageShowsFriendlyErrorForNonExistentUser()
    {
        // Arrange
        static void ConfigureTestServices(IServiceCollection services) =>
            services.AddSingleton<IGitHubClient>(new TestGitHubClientWithError());

        var client = _factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(ConfigureTestServices))
            .CreateClient();

        // Act
        var response = await client.GetAsync("/GitHubProfile/nonexistentuser123456789");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that error message is displayed
        Assert.Contains("alert alert-danger", content);
        Assert.Contains("not found", content);
        Assert.Contains("Please check the username", content);
        
        // Check that no profile content is displayed
        Assert.DoesNotContain("user-login", content);
    }

    [Fact]
    public async Task Get_GitHubProfilePageShowsProfileForValidUser()
    {
        // Arrange
        static void ConfigureTestServices(IServiceCollection services) =>
            services.AddSingleton<IGitHubClient>(new TestGitHubClientWithValidUser());

        var client = _factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(ConfigureTestServices))
            .CreateClient();

        // Act
        var response = await client.GetAsync("/GitHubProfile/validuser");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that profile content is displayed
        Assert.Contains("user-login", content);
        Assert.Contains("validuser", content);
        
        // Check that no error message is displayed
        Assert.DoesNotContain("alert alert-danger", content);
    }

    public class TestGitHubClientWithError : IGitHubClient
    {
        public Task<GitHubUserResult> GetUserAsync(string userName)
        {
            return Task.FromResult(GitHubUserResult.UserNotFound(userName));
        }
    }

    public class TestGitHubClientWithValidUser : IGitHubClient
    {
        public Task<GitHubUserResult> GetUserAsync(string userName)
        {
            var user = new GitHubUser
            {
                Login = userName,
                Name = "Test User",
                Company = "Test Company",
                AvatarUrl = "https://example.com/avatar.jpg"
            };
            return Task.FromResult(GitHubUserResult.Success(user));
        }
    }
}