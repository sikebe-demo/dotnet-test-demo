using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests;

public class GithubProfilePageTest : IClassFixture<EdgeFixture>
{
    private readonly BrowserFixture _browser;
    private readonly ITestOutputHelper _helper;
    private readonly GitHubProfilePage _githubProfilePage;

    public GithubProfilePageTest(EdgeFixture edgeFixture, ITestOutputHelper helper)
    {
        _browser = edgeFixture;
        _helper = helper;

        _browser.Driver.Navigate().GoToUrl(Constants.BaseUrl + "/GithubProfile");
        _githubProfilePage = new GitHubProfilePage(_browser.Driver, _helper);
    }

    [Fact]
    public void Should_Show_GitHub_Account()
    {
        // Arrange
        var userName = "msftgits";

        // Act
        _githubProfilePage.UserName.Clear();
        _githubProfilePage.UserName.SendKeys(userName);
        _githubProfilePage.ClickShowUserProfileButton();

        // Assert
        Assert.Equal(userName, _githubProfilePage.Login.Text);
        Assert.Equal("Microsoft GitHub User", _githubProfilePage.Name.Text);
        Assert.Equal("Microsoft", _githubProfilePage.Company.Text);
    }

    [Fact]
    public void Should_Show_Error_When_User_Not_Found()
    {
        // Arrange
        var userName = "thisuserdoesnotexist1234567890";

        // Act
        _githubProfilePage.UserName.Clear();
        _githubProfilePage.UserName.SendKeys(userName);
        _githubProfilePage.ClickShowUserProfileButton();

        // Assert
        Assert.NotNull(_githubProfilePage.ErrorBanner);
        Assert.Contains($"ユーザー '{userName}' は見つかりませんでした。", _githubProfilePage.ErrorBanner.Text);
    }
}
