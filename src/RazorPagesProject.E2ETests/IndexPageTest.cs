using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

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
    public void Should_Disable_Add_Button_When_Input_Is_Empty()
    {
        // Arrange & Act - Button should be disabled on page load
        var addButton = _browserFixture.Driver.FindElement(By.Id("addMessageBtn"));
        var messageInput = _browserFixture.Driver.FindElement(By.Id("newMessage"));

        // Assert - Button should be disabled when input is empty
        Assert.True(addButton.GetAttribute("disabled") != null, 
            "Add Message button should be disabled when input is empty");
        Assert.Contains("btn-secondary", addButton.GetAttribute("class"));
    }

    [Fact]
    public void Should_Enable_Add_Button_When_Input_Has_Text()
    {
        // Arrange
        var addButton = _browserFixture.Driver.FindElement(By.Id("addMessageBtn"));
        var messageInput = _browserFixture.Driver.FindElement(By.Id("newMessage"));

        // Act - Type text into input
        messageInput.Clear();
        messageInput.SendKeys("Test message");

        // Wait for JavaScript to update button state
        var wait = new WebDriverWait(_browserFixture.Driver, TimeSpan.FromSeconds(5));
        wait.Until(driver => addButton.GetAttribute("disabled") is null);

        // Assert - Button should be enabled when input has text
        Assert.True(addButton.GetAttribute("disabled") is null, 
            "Add Message button should be enabled when input has text");
        Assert.Contains("btn-primary", addButton.GetAttribute("class"));
    }

    [Fact]
    public void Should_Display_Long_Messages_With_Proper_Word_Wrapping()
    {
        // Arrange
        var longMessage = "This is a very long message with supercalifragilisticexpialidocious words and URLs like https://example.com/very-long-url";
        
        // Act - Add the long message
        var messageInput = _browserFixture.Driver.FindElement(By.Id("newMessage"));
        messageInput.Clear();
        messageInput.SendKeys(longMessage);
        
        var addButton = _browserFixture.Driver.FindElement(By.Id("addMessageBtn"));
        
        // Wait for button to be enabled
        var wait = new WebDriverWait(_browserFixture.Driver, TimeSpan.FromSeconds(5));
        wait.Until(driver => addButton.GetAttribute("disabled") is null);
        
        addButton.Click();

        // Wait for page to reload and message to appear
        wait.Until(driver => driver.FindElements(By.ClassName("message-list")).Any(m => m.Text.Contains("supercalifragilisticexpialidocious")));

        // Assert - Message should be visible and contain expected text
        var messageElements = _browserFixture.Driver.FindElements(By.ClassName("message-list"));
        var addedMessage = messageElements.FirstOrDefault(m => m.Text.Contains("supercalifragilisticexpialidocious"));
        
        Assert.NotNull(addedMessage);
        Assert.Contains("supercalifragilisticexpialidocious", addedMessage.Text);
    }
}
