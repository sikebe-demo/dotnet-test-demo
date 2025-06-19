using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class IndexPage(IWebDriver driver, ITestOutputHelper? helper) : PageObjectBase(driver, helper)
{
    public IWebElement NewMessage => Driver.FindElement(By.Name("Message.Text"));

    public async Task<IndexPage> AddMessageAsync(string message)
    {
        NewMessage.Clear();
        NewMessage.SendKeys(message);
        Driver.FindElement(By.Id("addMessageBtn")).Click();

        // Improved waiting logic to avoid StaleElementReferenceException
        await WaitForConditionAsync(() =>
        {
            try
            {
                // Re-fetch elements each time to avoid stale references
                var currentMessages = Driver.FindElements(By.ClassName("message-list"));
                return currentMessages.Any(m =>
                {
                    try
                    {
                        return m.Text == message;
                    }
                    catch (StaleElementReferenceException)
                    {
                        // If element is stale, re-fetch and try again
                        return false;
                    }
                });
            }
            catch (Exception)
            {
                return false;
            }
        }, TimeSpan.FromSeconds(15));

        return this;
    }

    public bool HasMessage(string expectedMessage)
    {
        try
        {
            // Always fetch fresh elements to avoid stale references
            var currentMessages = Driver.FindElements(By.ClassName("message-list"));
            return currentMessages.Any(m =>
            {
                try
                {
                    return m.Text == expectedMessage;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IndexPage ClickAddMessageButton()
    {
        Driver.FindElement(By.Id("addMessageBtn")).Click();
        return new IndexPage(Driver, Helper);
    }
}
