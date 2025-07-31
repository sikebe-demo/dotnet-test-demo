using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;
using OpenQA.Selenium;
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
    public async Task Should_Show_Confirmation_Dialog_When_Clear_All_Clicked()
    {
        // Arrange - Add a test message to ensure there's something to delete
        await _indexPage.AddMessageAsync("Test message for clear all");

        // Act - Click Clear All button but don't handle dialog yet
        _browserFixture.Driver.FindElement(By.Id("deleteAllBtn")).Click();

        // Assert - Check if confirmation dialog appears with correct message
        var confirmText = _indexPage.GetConfirmationDialogText();
        Assert.Contains("Are you sure you want to delete", confirmText);
        Assert.Contains("This action cannot be undone", confirmText);
    }

    [Fact]
    public async Task Should_Not_Delete_Messages_When_Cancel_Confirmation()
    {
        // Arrange
        var testMessage = "Message that should remain after cancel";
        await _indexPage.AddMessageAsync(testMessage);
        var initialMessageCount = _indexPage.GetMessageCount();

        // Act - Click Clear All and cancel the confirmation
        await _indexPage.ClickClearAllButtonWithConfirmationAsync(acceptConfirmation: false);

        // Assert - Messages should still be there
        Assert.True(_indexPage.HasMessage(testMessage), 
            "Message should still exist after canceling Clear All confirmation");
        Assert.Equal(initialMessageCount, _indexPage.GetMessageCount());
    }

    [Fact]
    public async Task Should_Delete_All_Messages_When_Accept_Confirmation()
    {
        // Arrange
        var testMessage = "Message that should be deleted";
        await _indexPage.AddMessageAsync(testMessage);
        
        // Act - Click Clear All and accept the confirmation
        await _indexPage.ClickClearAllButtonWithConfirmationAsync(acceptConfirmation: true);

        // Assert - Messages should be gone
        Assert.False(_indexPage.HasMessage(testMessage),
            "Message should be deleted after accepting Clear All confirmation");
        Assert.Equal(0, _indexPage.GetMessageCount());
    }

    [Fact]
    public async Task Should_Show_Message_Count_In_Confirmation_Dialog()
    {
        // Arrange - Add multiple messages
        await _indexPage.AddMessageAsync("Message 1");
        await _indexPage.AddMessageAsync("Message 2");
        var messageCount = _indexPage.GetMessageCount();

        // Act - Click Clear All button
        _browserFixture.Driver.FindElement(By.Id("deleteAllBtn")).Click();

        // Assert - Check that the confirmation dialog shows the correct count
        var confirmText = _indexPage.GetConfirmationDialogText();
        if (messageCount == 1)
        {
            Assert.Contains("delete 1 message", confirmText);
        }
        else
        {
            Assert.Contains($"delete {messageCount} messages", confirmText);
        }
    }
}
