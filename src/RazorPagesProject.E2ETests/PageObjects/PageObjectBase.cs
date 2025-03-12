using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class PageObjectBase
{
    public PageObjectBase(IWebDriver driver, ITestOutputHelper? helper = default)
    {
        Helper = helper;
        Driver = driver;
    }

    public IWebDriver Driver { get; }
    public ITestOutputHelper? Helper { get; set; }

    public GitHubProfilePage ClickGitHubProfileLink()
    {
        Driver.FindElement(By.LinkText("GitHub Profile")).Click();
        return new GitHubProfilePage(Driver, Helper);
    }
}
