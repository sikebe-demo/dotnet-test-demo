namespace RazorPagesProject.E2ETests.Configuration;

/// <summary>
/// Supported E2E test frameworks
/// </summary>
public enum TestFramework
{
    Selenium,
    Playwright
}

/// <summary>
/// Supported browser types
/// </summary>
public enum BrowserType
{
    Edge,
    Chrome,
    Firefox
}

/// <summary>
/// Configuration for E2E tests
/// </summary>
public class E2ETestConfiguration
{
    /// <summary>
    /// Which framework to use for testing
    /// </summary>
    public TestFramework Framework { get; set; } = TestFramework.Selenium;

    /// <summary>
    /// Which browser to use
    /// </summary>
    public BrowserType Browser { get; set; } = BrowserType.Edge;

    /// <summary>
    /// Whether to run in headless mode
    /// </summary>
    public bool Headless { get; set; } = true;

    /// <summary>
    /// Default timeout for operations
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Base URL for the application
    /// </summary>
    public string BaseUrl { get; set; } = "https://localhost:7072";

    /// <summary>
    /// Directory for downloads
    /// </summary>
    public string? DownloadDirectory { get; set; }

    /// <summary>
    /// Directory for logs and screenshots
    /// </summary>
    public string? LogDirectory { get; set; }
}