using RazorPagesProject.E2ETests.Configuration;
using RazorPagesProject.E2ETests.Fixtures;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.Demo;

/// <summary>
/// Demo test showing framework selection via configuration
/// </summary>
public class ConfigurationDemoTest : IDisposable
{
    private readonly UnifiedBrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;

    public ConfigurationDemoTest(ITestOutputHelper helper)
    {
        _helper = helper;
        _browserFixture = new UnifiedBrowserFixture();
    }

    [Fact]
    public void Should_Load_Configuration_Correctly()
    {
        // Arrange & Act
        var config = ConfigurationHelper.LoadConfiguration();
        
        // Assert - Verify configuration loading
        Assert.NotNull(config);
        Assert.True(Enum.IsDefined(typeof(TestFramework), config.Framework));
        Assert.True(Enum.IsDefined(typeof(BrowserType), config.Browser));
        
        _helper.WriteLine($"Framework: {config.Framework}");
        _helper.WriteLine($"Browser: {config.Browser}");
        _helper.WriteLine($"Headless: {config.Headless}");
        _helper.WriteLine($"Timeout: {config.Timeout}");
        _helper.WriteLine($"Base URL: {config.BaseUrl}");
    }

    [Fact]
    public async Task Should_Create_Driver_Based_On_Configuration()
    {
        // Arrange & Act
        var driver = await _browserFixture.GetDriverAsync();
        
        // Assert
        Assert.NotNull(driver);
        _helper.WriteLine($"Successfully created driver for framework: {_browserFixture.Framework}");
        
        // Test basic driver functionality
        var title = await driver.GetTitleAsync();
        _helper.WriteLine($"Initial page title: {title}");
    }

    public void Dispose()
    {
        _browserFixture?.Dispose();
    }
}