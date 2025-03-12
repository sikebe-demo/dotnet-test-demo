using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class IndexPage(IWebDriver driver, ITestOutputHelper? helper) : PageObjectBase(driver, helper)
{
    public IWebElement NewMessage => Driver.FindElement(By.Name("Message.Text"));

    public IList<IWebElement> Messages => Driver.FindElements(By.ClassName("message-list"));

    public IndexPage ClickAddMessageButton()
    {
        Driver.FindElement(By.Id("addMessageBtn")).Click();
        return new IndexPage(Driver, Helper);
    }
}
