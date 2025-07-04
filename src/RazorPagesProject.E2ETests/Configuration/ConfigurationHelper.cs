using Microsoft.Extensions.Configuration;

namespace RazorPagesProject.E2ETests.Configuration;

/// <summary>
/// Helper class to load E2E test configuration from various sources
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// Load configuration from appsettings.json and environment variables
    /// </summary>
    public static E2ETestConfiguration LoadConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables("E2E_")
            .Build();

        var config = new E2ETestConfiguration();

        // Load from configuration section
        var section = configuration.GetSection("E2ETests");
        if (section.Exists())
        {
            section.Bind(config);
        }

        // Override with environment variables if present
        if (Enum.TryParse<TestFramework>(Environment.GetEnvironmentVariable("E2E_FRAMEWORK"), out var framework))
        {
            config.Framework = framework;
        }

        if (Enum.TryParse<BrowserType>(Environment.GetEnvironmentVariable("E2E_BROWSER"), out var browser))
        {
            config.Browser = browser;
        }

        if (bool.TryParse(Environment.GetEnvironmentVariable("E2E_HEADLESS"), out var headless))
        {
            config.Headless = headless;
        }

        if (TimeSpan.TryParse(Environment.GetEnvironmentVariable("E2E_TIMEOUT"), out var timeout))
        {
            config.Timeout = timeout;
        }

        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL");
        if (!string.IsNullOrEmpty(baseUrl))
        {
            config.BaseUrl = baseUrl;
        }

        return config;
    }
}