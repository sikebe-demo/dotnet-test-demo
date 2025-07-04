using OpenQA.Selenium;

namespace RazorPagesProject.E2ETests.Abstractions;

/// <summary>
/// Common interface for browser automation frameworks (Selenium, Playwright)
/// </summary>
public interface IBrowserDriver : IDisposable
{
    /// <summary>
    /// Navigate to the specified URL
    /// </summary>
    Task NavigateAsync(string url);

    /// <summary>
    /// Get the current page title
    /// </summary>
    Task<string> GetTitleAsync();

    /// <summary>
    /// Get the current page URL
    /// </summary>
    Task<string> GetUrlAsync();

    /// <summary>
    /// Get the page source content
    /// </summary>
    Task<string> GetPageSourceAsync();

    /// <summary>
    /// Find a single element by selector
    /// </summary>
    Task<IElementLocator> FindElementAsync(By selector, TimeSpan? timeout = null);

    /// <summary>
    /// Find multiple elements by selector
    /// </summary>
    Task<IList<IElementLocator>> FindElementsAsync(By selector, TimeSpan? timeout = null);

    /// <summary>
    /// Check if an element exists
    /// </summary>
    Task<bool> ElementExistsAsync(By selector, TimeSpan? timeout = null);

    /// <summary>
    /// Wait for a condition to be true
    /// </summary>
    Task WaitForConditionAsync(Func<IBrowserDriver, Task<bool>> condition, TimeSpan? timeout = null);

    /// <summary>
    /// Take a screenshot of the current page
    /// </summary>
    Task<byte[]> TakeScreenshotAsync();

    /// <summary>
    /// Execute JavaScript code
    /// </summary>
    Task<object> ExecuteScriptAsync(string script, params object[] args);

    /// <summary>
    /// Get the underlying driver implementation for framework-specific operations
    /// </summary>
    object GetNativeDriver();
}