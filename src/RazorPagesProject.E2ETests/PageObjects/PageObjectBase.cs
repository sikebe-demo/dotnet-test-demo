using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class PageObjectBase(IWebDriver driver, ITestOutputHelper? helper = default)
{
    public IWebDriver Driver { get; } = driver;
    public ITestOutputHelper? Helper { get; set; } = helper;

    protected WebDriverWait CreateWait(TimeSpan? timeout = null)
    {
        return new WebDriverWait(Driver, timeout ?? TimeSpan.FromSeconds(10));
    }

    protected async Task WaitForConditionAsync(Func<bool> condition, TimeSpan? timeout = null, int pollInterval = 100)
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

    public GitHubProfilePage ClickGitHubProfileLink()
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));

        // Try to find the link by different possible texts (localized)
        IWebElement? githubProfileLink = null;

        try
        {
            // Try English first
            githubProfileLink = wait.Until(driver =>
                driver.FindElement(By.LinkText("GitHub Profile")));
        }
        catch (WebDriverTimeoutException)
        {
            try
            {
                // Try Japanese
                githubProfileLink = wait.Until(driver =>
                    driver.FindElement(By.LinkText("GitHub プロフィール")));
            }
            catch (WebDriverTimeoutException)
            {
                // Fall back to XPath that matches the asp-page attribute
                githubProfileLink = wait.Until(driver =>
                    driver.FindElement(By.XPath("//a[@asp-page='/GitHubProfile' or contains(@href, '/GitHubProfile')]")));
            }
        }

        githubProfileLink?.Click();
        return new GitHubProfilePage(Driver, Helper);
    }
}
