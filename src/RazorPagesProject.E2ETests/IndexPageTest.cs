using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests;

public class IndexPageTest : IClassFixture<EdgeFixture>
{
    private readonly BrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;
    private readonly IndexPage _topPage;

    public IndexPageTest(EdgeFixture browserFixture, ITestOutputHelper helper)
    {
        _browserFixture = browserFixture;
        _helper = helper;

        _browserFixture.Driver.Navigate().GoToUrl(Constants.BaseUrl);
        _topPage = new IndexPage(_browserFixture.Driver, _helper);
    }

    [Fact]
    public void Should_Add_New_Message()
    {
        // Arrange
        var newMessage = Guid.NewGuid().ToString();

        // Act
        _topPage.NewMessage.Clear();
        _topPage.NewMessage.SendKeys(newMessage);
        _topPage.ClickAddMessageButton();

        // Assert
        var message = _topPage.Messages.SingleOrDefault(m => m.Text == newMessage);
        Assert.NotNull(message);
    }

    [Fact]
    public void Should_Transition_to_GitHubProfilePage()
    {
        // Arrange, Act
        var githubProfilePage = _topPage.ClickGitHubProfileLink();

        // Assert
        Assert.Equal("GitHub Profile - RazorPagesProject", githubProfilePage.Driver.Title);
    }
}
