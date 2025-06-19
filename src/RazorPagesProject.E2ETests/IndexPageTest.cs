using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;
using OpenQA.Selenium.Support.UI;

namespace RazorPagesProject.E2ETests;

public class IndexPageTest : IClassFixture<EdgeFixture>
{
    private readonly BrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;
    private readonly IndexPage _indexPage;

    public IndexPageTest(EdgeFixture browserFixture, ITestOutputHelper helper)
    {
        _browserFixture = browserFixture;
        _helper = helper;

        // Navigate to English version to ensure consistent test behavior
        _browserFixture.Driver.Navigate().GoToUrl($"{Constants.BaseUrl}?culture=en");
        _indexPage = new IndexPage(_browserFixture.Driver, _helper);
    }

    [Fact]
    public async Task Should_Add_New_Message()
    {
        // Arrange
        var newMessage = Guid.NewGuid().ToString();

        // Act
        await _indexPage.AddMessageAsync(newMessage);

        // Assert
        Assert.True(_indexPage.HasMessage(newMessage),
            $"Expected message '{newMessage}' was not found on the page");
    }

    [Fact]
    public void Should_Transition_to_GitHubProfilePage()
    {
        // Arrange, Act
        var githubProfilePage = _indexPage.ClickGitHubProfileLink();

        // Wait for page to load and then check URL contains GitHubProfile
        var wait = new WebDriverWait(githubProfilePage.Driver, TimeSpan.FromSeconds(10));
        wait.Until(driver => driver.Url.Contains("GithubProfile"));

        // Assert - Check that we're on the GitHub Profile page (more flexible)
        var title = githubProfilePage.Driver.Title;
        Assert.True(
            title.Contains("GitHub") && title.Contains("RazorPagesProject"),
            $"Expected GitHub Profile page title containing 'GitHub' and 'RazorPagesProject' but got: {title}");
    }
}
