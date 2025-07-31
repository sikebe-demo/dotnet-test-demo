using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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

    public async Task<IndexPage> ClickClearAllButtonWithConfirmationAsync(bool acceptConfirmation = true)
    {
        // Click the Clear All button
        Driver.FindElement(By.Id("deleteAllBtn")).Click();

        // Wait for and handle the confirmation dialog
        await WaitForConditionAsync(() =>
        {
            try
            {
                var alert = Driver.SwitchTo().Alert();
                if (acceptConfirmation)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }, TimeSpan.FromSeconds(10));

        // If we accepted the confirmation, wait for the page to reload
        if (acceptConfirmation)
        {
            await WaitForConditionAsync(() =>
            {
                try
                {
                    // After deletion, the messages list should be empty or refreshed
                    Driver.FindElement(By.Id("deleteAllBtn"));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }, TimeSpan.FromSeconds(10));
        }

        return this;
    }

    public int GetMessageCount()
    {
        try
        {
            return Driver.FindElements(By.ClassName("message-list")).Count;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public string GetConfirmationDialogText()
    {
        try
        {
            var alert = Driver.SwitchTo().Alert();
            var text = alert.Text;
            alert.Dismiss(); // Dismiss the alert to prevent it from interfering
            return text ?? string.Empty;
        }
        catch (NoAlertPresentException)
        {
            return string.Empty;
        }
    }
}
