using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class PageObjectBase(IWebDriver driver, ITestOutputHelper? helper = default)
{
    public IWebDriver Driver { get; } = driver;
    public ITestOutputHelper? Helper { get; set; } = helper;

    public GitHubProfilePage ClickGitHubProfileLink()
    {
        Driver.FindElement(By.LinkText("GitHub Profile")).Click();
        return new GitHubProfilePage(Driver, Helper);
    }
}
