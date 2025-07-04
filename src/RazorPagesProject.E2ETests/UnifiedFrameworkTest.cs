using RazorPagesProject.E2ETests.Configuration;
using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests;

/// <summary>
/// Test class demonstrating the unified E2E testing framework supporting both Selenium and Playwright
/// </summary>
public class UnifiedFrameworkTest : IClassFixture<UnifiedBrowserFixture>
{
    private readonly UnifiedBrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;

    public UnifiedFrameworkTest(UnifiedBrowserFixture browserFixture, ITestOutputHelper helper)
    {
        _browserFixture = browserFixture;
        _helper = helper;
    }

    [Fact]
    public async Task Should_Work_With_Both_Selenium_And_Playwright()
    {
        // Arrange - Get unified driver
        var driver = await _browserFixture.GetDriverAsync();
        var baseUrl = _browserFixture.Configuration.BaseUrl;
        
        _helper.WriteLine($"Testing with framework: {_browserFixture.Framework}");
        _helper.WriteLine($"Testing with browser: {_browserFixture.Browser}");
        
        // Navigate to the application
        await driver.NavigateAsync($"{baseUrl}?culture=en");
        
        // Act - Create page object and test basic functionality
        var indexPage = new UnifiedIndexPage(driver, _helper);
        var newMessage = $"Test message from {_browserFixture.Framework} at {DateTime.Now:HH:mm:ss}";
        
        await indexPage.AddMessageAsync(newMessage);
        
        // Assert - Verify the message was added
        var hasMessage = await indexPage.HasMessageAsync(newMessage);
        Assert.True(hasMessage, $"Expected message '{newMessage}' was not found on the page when using {_browserFixture.Framework}");
        
        _helper.WriteLine($"Successfully tested with {_browserFixture.Framework} framework");
    }

    [Fact]
    public async Task Should_Navigate_To_GitHub_Profile_Page()
    {
        // Arrange
        var driver = await _browserFixture.GetDriverAsync();
        var baseUrl = _browserFixture.Configuration.BaseUrl;
        
        await driver.NavigateAsync($"{baseUrl}?culture=en");
        var indexPage = new UnifiedIndexPage(driver, _helper);
        
        // Act
        var githubProfilePage = await indexPage.ClickGitHubProfileLinkAsync();
        
        // Assert - Check that we're on the GitHub Profile page
        var url = await driver.GetUrlAsync();
        Assert.Contains("GitHubProfile", url);
        
        var title = await driver.GetTitleAsync();
        Assert.True(
            title.Contains("GitHub") && title.Contains("RazorPagesProject"),
            $"Expected GitHub Profile page title containing 'GitHub' and 'RazorPagesProject' but got: {title} when using {_browserFixture.Framework}");
        
        _helper.WriteLine($"Successfully navigated to GitHub Profile page with {_browserFixture.Framework} framework");
    }
}