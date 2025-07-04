using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.Abstractions;

/// <summary>
/// Base interface for Page Objects using unified driver abstraction
/// </summary>
public interface IPageObjectBase
{
    /// <summary>
    /// The unified browser driver
    /// </summary>
    IBrowserDriver Driver { get; }

    /// <summary>
    /// Test output helper for logging
    /// </summary>
    ITestOutputHelper? Helper { get; }

    /// <summary>
    /// Wait for a condition with timeout
    /// </summary>
    Task WaitForConditionAsync(Func<bool> condition, TimeSpan? timeout = null, int pollInterval = 100);
}