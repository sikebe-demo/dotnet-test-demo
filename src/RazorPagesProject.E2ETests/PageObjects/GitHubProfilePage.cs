using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class GitHubProfilePage(IWebDriver driver, ITestOutputHelper? helper) : PageObjectBase(driver, helper)
{
    public IWebElement UserName => Driver.FindElement(By.Id("Input_UserName"));
    public IWebElement Login => Driver.FindElement(By.Id("user-login"));
    public IWebElement Name => Driver.FindElement(By.Id("name"));
    public IWebElement Company => Driver.FindElement(By.Id("company"));
    public IWebElement? ErrorBanner => Driver.FindElements(By.Id("error-banner")).FirstOrDefault();

    public GitHubProfilePage ClickShowUserProfileButton()
    {
        Driver.FindElement(By.XPath("//*[@id=\"user-profile\"]//button")).Click();
        return new GitHubProfilePage(Driver, Helper);
    }
}
