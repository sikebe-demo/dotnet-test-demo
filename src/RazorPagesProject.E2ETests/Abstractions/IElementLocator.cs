namespace RazorPagesProject.E2ETests.Abstractions;

/// <summary>
/// Common interface for element operations across different frameworks
/// </summary>
public interface IElementLocator
{
    /// <summary>
    /// Click the element
    /// </summary>
    Task ClickAsync();

    /// <summary>
    /// Send keys to the element (for input fields)
    /// </summary>
    Task SendKeysAsync(string text);

    /// <summary>
    /// Clear the element content (for input fields)
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Get the text content of the element
    /// </summary>
    Task<string> GetTextAsync();

    /// <summary>
    /// Get an attribute value from the element
    /// </summary>
    Task<string> GetAttributeAsync(string attributeName);

    /// <summary>
    /// Check if the element is displayed
    /// </summary>
    Task<bool> IsDisplayedAsync();

    /// <summary>
    /// Check if the element is enabled
    /// </summary>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Check if the element is selected (for checkboxes/radio buttons)
    /// </summary>
    Task<bool> IsSelectedAsync();

    /// <summary>
    /// Get the underlying element object for framework-specific operations
    /// </summary>
    object GetNativeElement();
}