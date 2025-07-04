using RazorPagesProject.E2ETests.Configuration;
using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.FrameworkSpecific;

/// <summary>
/// Test class that explicitly uses Selenium framework
/// </summary>
public class SeleniumSpecificTest : IDisposable
{
    private readonly UnifiedBrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;

    public SeleniumSpecificTest(ITestOutputHelper helper)
    {
        _helper = helper;
        
        // Override environment variables to force Selenium
        Environment.SetEnvironmentVariable("E2E_FRAMEWORK", "Selenium");
        Environment.SetEnvironmentVariable("E2E_BROWSER", "Edge");
        Environment.SetEnvironmentVariable("E2E_HEADLESS", "true");
        Environment.SetEnvironmentVariable("E2E_BASE_URL", "https://localhost:7072");
        
        _browserFixture = new UnifiedBrowserFixture();
    }

    [Fact]
    public async Task Should_Work_With_Selenium_Framework()
    {
        // Arrange
        var driver = await _browserFixture.GetDriverAsync();
        
        Assert.Equal(TestFramework.Selenium, _browserFixture.Framework);
        _helper.WriteLine($"✅ Testing with Selenium framework using {_browserFixture.Browser} browser");
        
        // Act - Navigate and test basic functionality
        await driver.NavigateAsync($"{_browserFixture.Configuration.BaseUrl}?culture=en");
        
        var indexPage = new UnifiedIndexPage(driver, _helper);
        var testMessage = $"Selenium test message at {DateTime.Now:HH:mm:ss}";
        
        await indexPage.AddMessageAsync(testMessage);
        
        // Assert
        var hasMessage = await indexPage.HasMessageAsync(testMessage);
        Assert.True(hasMessage, $"Expected message '{testMessage}' was not found on the page");
        
        _helper.WriteLine("✅ Selenium test completed successfully");
    }

    public void Dispose()
    {
        // Clean up environment variables
        Environment.SetEnvironmentVariable("E2E_FRAMEWORK", null);
        Environment.SetEnvironmentVariable("E2E_BROWSER", null);
        Environment.SetEnvironmentVariable("E2E_HEADLESS", null);
        Environment.SetEnvironmentVariable("E2E_BASE_URL", null);
        
        _browserFixture?.Dispose();
    }
}