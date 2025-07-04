using RazorPagesProject.E2ETests.Abstractions;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

/// <summary>
/// Unified base class for Page Objects that works with both Selenium and Playwright
/// </summary>
public class UnifiedPageObjectBase : IPageObjectBase
{
    public IBrowserDriver Driver { get; }
    public ITestOutputHelper? Helper { get; set; }

    public UnifiedPageObjectBase(IBrowserDriver driver, ITestOutputHelper? helper = null)
    {
        Driver = driver ?? throw new ArgumentNullException(nameof(driver));
        Helper = helper;
    }

    /// <summary>
    /// Wait for a condition with timeout and polling
    /// </summary>
    public async Task WaitForConditionAsync(Func<Task<bool>> condition, TimeSpan? timeout = null, int pollInterval = 100)
    {
        var endTime = DateTime.UtcNow.Add(timeout ?? TimeSpan.FromSeconds(10));

        while (DateTime.UtcNow < endTime)
        {
            if (await condition())
                return;

            await Task.Delay(pollInterval);
        }

        throw new TimeoutException("Condition was not met within the specified timeout.");
    }

    /// <summary>
    /// Wait for a synchronous condition with timeout and polling
    /// </summary>
    public async Task WaitForConditionAsync(Func<bool> condition, TimeSpan? timeout = null, int pollInterval = 100)
    {
        var endTime = DateTime.UtcNow.Add(timeout ?? TimeSpan.FromSeconds(10));

        while (DateTime.UtcNow < endTime)
        {
            if (condition())
                return;

            await Task.Delay(pollInterval);
        }

        throw new TimeoutException("Condition was not met within the specified timeout.");
    }

    /// <summary>
    /// Navigate to GitHub Profile page from any page - works with both frameworks
    /// </summary>
    public async Task<UnifiedGitHubProfilePage> ClickGitHubProfileLinkAsync()
    {
        try
        {
            // Try to find the link by different possible texts (localized)
            IElementLocator? githubProfileLink = null;

            try
            {
                // Try English first
                githubProfileLink = await Driver.FindElementAsync(OpenQA.Selenium.By.LinkText("GitHub Profile"));
            }
            catch
            {
                try
                {
                    // Try Japanese
                    githubProfileLink = await Driver.FindElementAsync(OpenQA.Selenium.By.LinkText("GitHub プロフィール"));
                }
                catch
                {
                    // Fall back to XPath that matches the asp-page attribute
                    githubProfileLink = await Driver.FindElementAsync(OpenQA.Selenium.By.XPath("//a[@asp-page='/GitHubProfile' or contains(@href, '/GitHubProfile')]"));
                }
            }

            await githubProfileLink.ClickAsync();
            return new UnifiedGitHubProfilePage(Driver, Helper);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to click GitHub Profile link: {ex.Message}", ex);
        }
    }
}