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
        wait.Until(driver => driver.Url.Contains("GitHubProfile"));

        // Assert - Check that we're on the GitHub Profile page (more flexible)
        var title = githubProfilePage.Driver.Title;
        Assert.True(
            title.Contains("GitHub") && title.Contains("RazorPagesProject"),
            $"Expected GitHub Profile page title containing 'GitHub' and 'RazorPagesProject' but got: {title}");
    }

    [Fact]
    public async Task Should_Show_Confirmation_Dialog_When_Clear_All_Is_Clicked()
    {
        // Arrange
        var testMessage = "Test message for confirmation dialog";
        await _indexPage.AddMessageAsync(testMessage);
        
        // Verify message was added
        Assert.True(_indexPage.HasMessage(testMessage), 
            $"Test message '{testMessage}' should be present before testing Clear All");

        // Act - Click Clear All and cancel
        var dialogShown = _indexPage.ClickClearAllButtonAndCancel();

        // Assert - Confirmation dialog should be shown and messages should remain
        Assert.True(dialogShown, "Confirmation dialog should be displayed when Clear All is clicked");
        Assert.True(_indexPage.HasMessage(testMessage), 
            "Messages should remain when confirmation dialog is cancelled");
    }

    [Fact]
    public async Task Should_Delete_All_Messages_When_Confirmation_Dialog_Is_Accepted()
    {
        // Arrange
        var testMessage = "Test message to be deleted";
        await _indexPage.AddMessageAsync(testMessage);
        
        // Verify message was added
        Assert.True(_indexPage.HasMessage(testMessage), 
            $"Test message '{testMessage}' should be present before testing Clear All");
        var initialMessageCount = _indexPage.GetMessageCount();
        Assert.True(initialMessageCount > 0, "Should have at least one message before testing Clear All");

        // Act - Click Clear All and accept the confirmation
        var dialogShown = _indexPage.ClickClearAllButtonAndAccept();

        // Assert - Confirmation dialog should be shown and all messages should be deleted
        Assert.True(dialogShown, "Confirmation dialog should be displayed when Clear All is clicked");
        
        // Wait for page reload after form submission
        var wait = new WebDriverWait(_browserFixture.Driver, TimeSpan.FromSeconds(10));
        wait.Until(driver => _indexPage.GetMessageCount() == 0);
        
        var finalMessageCount = _indexPage.GetMessageCount();
        Assert.Equal(0, finalMessageCount);
    }
}
