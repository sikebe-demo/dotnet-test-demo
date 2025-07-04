using RazorPagesProject.E2ETests.Configuration;
using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.Updated;

/// <summary>
/// Updated Index Page tests using the unified framework
/// Demonstrates migration from original Selenium-only approach
/// </summary>
public class UnifiedIndexPageTest : IClassFixture<UnifiedBrowserFixture>
{
    private readonly UnifiedBrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;

    public UnifiedIndexPageTest(UnifiedBrowserFixture browserFixture, ITestOutputHelper helper)
    {
        _browserFixture = browserFixture;
        _helper = helper;
    }

    [Fact]
    public async Task Should_Add_New_Message_With_Unified_Framework()
    {
        // Arrange
        var driver = await _browserFixture.GetDriverAsync();
        var baseUrl = _browserFixture.Configuration.BaseUrl;
        
        _helper.WriteLine($"Testing with framework: {_browserFixture.Framework}");
        
        // Navigate to English version to ensure consistent test behavior
        await driver.NavigateAsync($"{baseUrl}?culture=en");
        var indexPage = new UnifiedIndexPage(driver, _helper);
        
        var newMessage = Guid.NewGuid().ToString();

        // Act
        await indexPage.AddMessageAsync(newMessage);

        // Assert
        var hasMessage = await indexPage.HasMessageAsync(newMessage);
        Assert.True(hasMessage, $"Expected message '{newMessage}' was not found on the page using {_browserFixture.Framework}");
        
        _helper.WriteLine($"✅ Successfully added and verified message with {_browserFixture.Framework}");
    }

    [Fact]
    public async Task Should_Transition_to_GitHubProfilePage_With_Unified_Framework()
    {
        // Arrange
        var driver = await _browserFixture.GetDriverAsync();
        var baseUrl = _browserFixture.Configuration.BaseUrl;
        
        await driver.NavigateAsync($"{baseUrl}?culture=en");
        var indexPage = new UnifiedIndexPage(driver, _helper);

        // Act
        var githubProfilePage = await indexPage.ClickGitHubProfileLinkAsync();

        // Wait for page to load and then check URL contains GitHubProfile
        await driver.WaitForConditionAsync(async (d) => 
        {
            var url = await d.GetUrlAsync();
            return url.Contains("GitHubProfile");
        });

        // Assert - Check that we're on the GitHub Profile page
        var currentUrl = await driver.GetUrlAsync();
        Assert.Contains("GitHubProfile", currentUrl);
        
        var title = await driver.GetTitleAsync();
        Assert.True(
            title.Contains("GitHub") && title.Contains("RazorPagesProject"),
            $"Expected GitHub Profile page title containing 'GitHub' and 'RazorPagesProject' but got: {title} using {_browserFixture.Framework}");
        
        _helper.WriteLine($"✅ Successfully navigated to GitHub Profile page with {_browserFixture.Framework}");
    }
}