using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests;

public class GitHubProfilePageTest : IClassFixture<EdgeFixture>
{
    private readonly BrowserFixture _browser;
    private readonly ITestOutputHelper _helper;
    private readonly GitHubProfilePage _gitHubProfilePage;

    public GitHubProfilePageTest(EdgeFixture edgeFixture, ITestOutputHelper helper)
    {
        _browser = edgeFixture;
        _helper = helper;

        _browser.Driver.Navigate().GoToUrl(Constants.BaseUrl + "/GitHubProfile");
        _gitHubProfilePage = new GitHubProfilePage(_browser.Driver, _helper);
    }

    [Fact]
    public void Should_Show_GitHub_Account()
    {
        // Arrange
        var userName = "msftgits";

        // Act
        _gitHubProfilePage.UserNameInput.Clear();
        _gitHubProfilePage.UserNameInput.SendKeys(userName);
        _gitHubProfilePage.ClickShowUserProfileButton();

        // Assert
        Assert.Equal(userName, _gitHubProfilePage.Login.Text);
        Assert.Equal("Microsoft GitHub User", _gitHubProfilePage.Name.Text);
        Assert.Equal("Microsoft", _gitHubProfilePage.Company.Text);
    }
}
