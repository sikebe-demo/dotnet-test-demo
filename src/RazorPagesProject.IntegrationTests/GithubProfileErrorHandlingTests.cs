using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RazorPagesProject.Pages;
using RazorPagesProject.Services;
using System.Net;
using Xunit;

namespace RazorPagesProject.IntegrationTests;

public class GithubProfileErrorHandlingTests
{
    private readonly IStringLocalizer<GithubProfileModel> _localizer;

    public GithubProfileErrorHandlingTests()
    {
        // Create a mock localizer for testing
        var localizerMock = new MockStringLocalizer();
        _localizer = localizerMock;
    }

    [Fact]
    public async Task OnGetAsync_NonExistentUser_ReturnsPageWithErrorMessage()
    {
        // Arrange
        var mockClient = new MockGithubClient();
        var pageModel = new GithubProfileModel(mockClient, _localizer)
        {
            Input = new GithubProfileModel.InputModel()
        };
        pageModel.ModelState.Clear();

        // Act
        var result = await pageModel.OnGetAsync("nonexistentuser");

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(pageModel.GithubUser);
        Assert.False(pageModel.ModelState.IsValid);
        Assert.True(pageModel.ModelState.ContainsKey(string.Empty));
        Assert.Contains("not found", pageModel.ModelState[string.Empty]!.Errors[0].ErrorMessage.ToLower());
    }

    [Fact]
    public async Task OnGetAsync_ValidUser_ReturnsPageWithUserData()
    {
        // Arrange
        var mockClient = new MockGithubClient();
        var pageModel = new GithubProfileModel(mockClient, _localizer)
        {
            Input = new GithubProfileModel.InputModel()
        };
        pageModel.ModelState.Clear();

        // Act
        var result = await pageModel.OnGetAsync("validuser");

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(pageModel.GithubUser);
        Assert.True(pageModel.ModelState.IsValid);
        Assert.Equal("validuser", pageModel.GithubUser.Login);
    }

    [Fact]
    public async Task OnGetAsync_HttpRequestException_ReturnsPageWithErrorMessage()
    {
        // Arrange
        var mockClient = new MockGithubClientWithException();
        var pageModel = new GithubProfileModel(mockClient, _localizer)
        {
            Input = new GithubProfileModel.InputModel()
        };
        pageModel.ModelState.Clear();

        // Act
        var result = await pageModel.OnGetAsync("someuser");

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(pageModel.GithubUser);
        Assert.False(pageModel.ModelState.IsValid);
        Assert.True(pageModel.ModelState.ContainsKey(string.Empty));
        Assert.Contains("error occurred", pageModel.ModelState[string.Empty]!.Errors[0].ErrorMessage.ToLower());
    }

    [Fact]
    public async Task OnGetAsync_EmptyUserName_ReturnsPageWithoutError()
    {
        // Arrange
        var mockClient = new MockGithubClient();
        var pageModel = new GithubProfileModel(mockClient, _localizer)
        {
            Input = new GithubProfileModel.InputModel()
        };
        pageModel.ModelState.Clear();

        // Act
        var result = await pageModel.OnGetAsync("");

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(pageModel.GithubUser);
        Assert.True(pageModel.ModelState.IsValid);
    }

    private class MockGithubClient : IGithubClient
    {
        public Task<GitHubUser?> GetUserAsync(string userName)
        {
            if (userName == "validuser")
            {
                return Task.FromResult<GitHubUser?>(new GitHubUser
                {
                    Login = "validuser",
                    Name = "Valid User",
                    Company = "Test Company"
                });
            }
            return Task.FromResult<GitHubUser?>(null);
        }
    }

    private class MockGithubClientWithException : IGithubClient
    {
        public Task<GitHubUser?> GetUserAsync(string userName)
        {
            throw new HttpRequestException("Network error");
        }
    }

    private class MockStringLocalizer : IStringLocalizer<GithubProfileModel>
    {
        public LocalizedString this[string name] => new(name, GetLocalizedValue(name));

        public LocalizedString this[string name, params object[] arguments] => 
            new(name, string.Format(GetLocalizedValue(name), arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            yield return new LocalizedString("UserNotFound", "User \"{0}\" not found.");
            yield return new LocalizedString("ErrorFetchingUser", "An error occurred while fetching user information.");
        }

        private string GetLocalizedValue(string name)
        {
            return name switch
            {
                "UserNotFound" => "User \"{0}\" not found.",
                "ErrorFetchingUser" => "An error occurred while fetching user information.",
                _ => name
            };
        }
    }
}