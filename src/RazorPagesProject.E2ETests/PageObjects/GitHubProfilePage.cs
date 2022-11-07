using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class GitHubProfilePage : PageObjectBase
{
    public GitHubProfilePage(IWebDriver driver, ITestOutputHelper? helper) : base(driver, helper)
    {
    }

    public IWebElement UserName => Driver.FindElement(By.Id("Input_UserName"));
    public IWebElement Login => Driver.FindElement(By.Id("user-login"));
    public IWebElement Name => Driver.FindElement(By.Id("name"));
    public IWebElement Company => Driver.FindElement(By.Id("company"));
    
    public GitHubProfilePage ClickShowUserProfileButton()
    {
        Driver.FindElement(By.XPath("//*[@id=\"user-profile\"]//button")).Click();
        return new GitHubProfilePage(Driver, Helper);
    }
}
