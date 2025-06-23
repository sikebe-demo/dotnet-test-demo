using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesProject.IntegrationTests.Helpers;
using RazorPagesProject.Services;
using Xunit;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class GitHubProfileErrorTests :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GitHubProfileErrorTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_GitHubProfilePageShowsErrorForNonExistentUser()
    {
        // Arrange
        static void ConfigureTestServices(IServiceCollection services) =>
            services.AddSingleton<IGitHubClient>(new TestGitHubClientWithError());

        var client = _factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(ConfigureTestServices))
            .CreateClient();

        // Act
        var response = await client.GetAsync("/GitHubProfile/nonexistentuser");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        // Check for the presence of error message (case insensitive)
        Assert.True(content.Contains("not found", StringComparison.OrdinalIgnoreCase) || 
                   content.Contains("GitHub user", StringComparison.OrdinalIgnoreCase), 
                   $"Expected error message not found in response. Content preview: {content.Substring(0, Math.Min(500, content.Length))}");
        
        // Ensure no profile elements are present when there's an error
        Assert.DoesNotContain("id=\"user-login\"", content);
        Assert.DoesNotContain("id=\"name\"", content);
        Assert.DoesNotContain("id=\"company\"", content);
    }

    public class TestGitHubClientWithError : IGitHubClient
    {
        public Task<GitHubUser?> GetUserAsync(string userName)
        {
            if (userName == "user" || userName == "octocat")
            {
                return Task.FromResult<GitHubUser?>(
                    new GitHubUser
                    {
                        Login = userName,
                        Company = "Contoso Blockchain",
                        Name = "John Doe"
                    });
            }
            else
            {
                return Task.FromResult<GitHubUser?>(null);
            }
        }
    }
}