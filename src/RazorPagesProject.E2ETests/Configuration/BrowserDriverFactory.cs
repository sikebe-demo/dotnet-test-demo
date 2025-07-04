using RazorPagesProject.E2ETests.Abstractions;
using RazorPagesProject.E2ETests.Adapters;

namespace RazorPagesProject.E2ETests.Configuration;

/// <summary>
/// Factory for creating browser drivers based on configuration
/// </summary>
public class BrowserDriverFactory
{
    private readonly E2ETestConfiguration _configuration;

    public BrowserDriverFactory(E2ETestConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Create a browser driver instance based on the configuration
    /// </summary>
    public async Task<IBrowserDriver> CreateDriverAsync()
    {
        return _configuration.Framework switch
        {
            TestFramework.Selenium => await CreateSeleniumDriverAsync(),
            TestFramework.Playwright => await CreatePlaywrightDriverAsync(),
            _ => throw new NotSupportedException($"Framework {_configuration.Framework} is not supported")
        };
    }

    private async Task<IBrowserDriver> CreateSeleniumDriverAsync()
    {
        var adapter = new SeleniumBrowserDriverAdapter(_configuration);
        await adapter.InitializeAsync();
        return adapter;
    }

    private async Task<IBrowserDriver> CreatePlaywrightDriverAsync()
    {
        var adapter = new PlaywrightBrowserDriverAdapter(_configuration);
        await adapter.InitializeAsync();
        return adapter;
    }
}